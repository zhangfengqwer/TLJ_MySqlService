﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using NhInterMySQL.Model;
using TLJCommon;
using NhInterMySQL.Model;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_BuyYuanBao)]
    public class BuyYuanBaoHandler : BaseHandler
    {
        private HashSet<string> hashSet = new HashSet<string>();

        public override string OnResponse(string data)
        {
            BuyYuanBaoReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<BuyYuanBaoReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn($"传入的参数有误:{data} \n{e}");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string uid = defaultReqData.uid;
            string orderid = defaultReqData.order_id;
            int goodId = defaultReqData.goods_id;
            int num = defaultReqData.goods_num;
            string account = defaultReqData.account;
            string password = defaultReqData.password;
            float price = defaultReqData.price;

            if (!MySqlService.AdminAccount.Equals(account) || !MySqlService.AdminPassWord.Equals(password))
            {
                MySqlService.log.Warn("账号错误");
                return null;
            }

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || num == 0 || price == 0 ||
                string.IsNullOrWhiteSpace(orderid))
            {
                MySqlService.log.Warn($"字段有空,{defaultReqData}");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add(MyCommon.UID, uid);

            //判断订单重复
            if (!hashSet.Add(orderid))
            {
                MySqlService.log.Warn($"有订单重复,{orderid}");
                OperatorFail(_responseData);
                return _responseData.ToString();
            }

            BuyYuanBaoSql(goodId, num, uid, price, orderid, _responseData);
            return _responseData.ToString();
        }

        public void BuyYuanBaoSql(int goodId, int num, string uid, float price, string orderid, JObject responseData)
        {
            Goods goods = NHibernateHelper.goodsManager.GetGoods(goodId);
            bool IsSuccess = false;

            if (goods != null)
            {
                IsSuccess = BuyYuanbao(goods, num, price, orderid, uid);
            }
            if (IsSuccess)
            {
                //加赠元宝
                AddExtraYuanBao(uid, goods);
                //首充礼包
                AddFirstRechargeGift();
                OperatorSuccess(responseData);
            }
            else
            {
                string msg =
                    $"购买元宝失败，uid: {uid},goodid: {goodId},num: {num},price：{price},goodsprice:{goods?.price * num},orderid: {orderid}";
                MySqlService.log.Warn(msg);
                LogUtil.Log(uid, MyCommon.OpType.BUYYUANBAO, msg);
                OperatorFail(responseData);
            }
        }

        private void AddFirstRechargeGift()
        {
        }

        /// <summary>
        /// 加赠元宝
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="goods"></param>
        private void AddExtraYuanBao(string uid, Goods goods)
        {
            //更新userRecharge数据
            var userRecharge = NHibernateHelper.userRechargeManager.GetUserRecharge(uid, goods.goods_id);
            if (userRecharge == null)
            {
                userRecharge = ModelFactory.CreateUserRecharge(uid, goods.goods_id);
            }
            //首次充值，加送元宝
            if (userRecharge.recharge_count == 0)
            {
                if (MySqlUtil.AddProp(uid, goods.extra_reward))
                {
                    LogUtil.Log(uid, MyCommon.OpType.EXTRA_YUANBAO,
                        $"购买了{goods.goods_id},{goods.goods_name},加赠了{goods.extra_reward}");
                }
                else
                {
                    MySqlService.log.Warn($"{uid},{goods.goods_id} 加赠元宝失败，数据库内部错误");
                }
            }
            userRecharge.recharge_count++;
            NHibernateHelper.userRechargeManager.Update(userRecharge);
        }

        private bool BuyYuanbao(Goods goods, int num, float price, string orderid, string uid)
        {
            bool IsSuccess = false;
            //需要的人民币
            int sumPrice = goods.price * num;

            if (sumPrice != price)
            {
                return false;
            }

            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
            //只能用人民币
            if (goods.money_type == 3)
            {
                MySqlService.log.Info("添加元宝");
                if (AddYuanbao(goods, userInfo, num, price, orderid))
                {
                    IsSuccess = true;
                }
            }
            return IsSuccess;
        }

        private bool AddYuanbao(Goods goods, UserInfo userInfo, int num, float price, string orderid)
        {
            string[] strings = goods.props.Split(':');
            int propId = Convert.ToInt32(strings[0]);
            int propNum = Convert.ToInt32(strings[1]);
            if (propId == 2)
            {
                userInfo.YuanBao += propNum * num;
                //之前的vip等级
                int vipFirstLevel = VipUtil.GetVipLevel(userInfo.RechargeVip);

                userInfo.RechargeVip += goods.price * num;
                //充值之后的vip等级
                int vipLevel = VipUtil.GetVipLevel(userInfo.RechargeVip);
                if (NHibernateHelper.userInfoManager.Update(userInfo))
                {
                    var format = string.Format("花费了{0}元，购买了{1}元宝,订单号：{2}", price, propNum * num, orderid);
                    LogUtil.Log(userInfo.Uid, MyCommon.OpType.BUYYUANBAO, format);

                    var result = vipLevel - vipFirstLevel;
                    //vip等级发生
                    if (result > 0)
                    {
                        for (int i = vipFirstLevel; i < vipLevel; i++)
                        {
                            var reward = $"1:{MySqlService.VipDatas[i].vipOnce.goldNum};{MySqlService.VipDatas[i].vipOnce.prop}";

                            //发送vip一次福利
                            SendEmailUtil.SendEmail(userInfo.Uid, $"贵族一次性福利", $"恭喜您升级到贵族{i + 1}", reward);

                            var temp = string.Format($"恭喜你升级到Vip{i + 1},发送邮件奖励:{reward}");
                            LogUtil.Log(userInfo.Uid, MyCommon.OpType.VIP_ONCE_REWARD, temp);
                        }
                    }

                    //首充礼包
                    if (userInfo.RechargeVip >= 6)
                    {
                        var commonConfig = NHibernateHelper.commonConfigManager.GetByUid(userInfo.Uid);
                        if (commonConfig == null)
                        {
                            commonConfig = ModelFactory.CreateConfig(userInfo.Uid);
                        }
                        if (commonConfig.first_recharge_gift == 0)
                        {
                            SendEmailUtil.SendEmail(userInfo.Uid, "首充礼包", "恭喜您获得首充奖励", "1:30000;107:30;102:3;106:3;110:2");
                            commonConfig.first_recharge_gift = 1;
                            NHibernateHelper.commonConfigManager.Update(commonConfig);
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}