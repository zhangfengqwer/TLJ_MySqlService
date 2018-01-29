using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_BuyGoods)]
    class BuyGoodsHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            BuyGoodReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<BuyGoodReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string uid = defaultReqData.uid;
            int goodId = defaultReqData.goods_id;
            int num = defaultReqData.goods_num;
            int money_type = defaultReqData.money_type;
            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || num == 0 || money_type < 1 ||
                money_type > 4)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            BuyGoodsSql(goodId, num, uid,money_type, _responseData);
            return _responseData.ToString();
        }

        private void BuyGoodsSql(int goodId, int num, string uid, int moneyType, JObject responseData)
        {
            Goods goods = NHibernateHelper.goodsManager.GetGoods(goodId);
            bool IsSuccess = false;
            if (goods != null)
            {
                switch (goods.goods_type)
                {
                    //购买的是金币
                    case 1:
                        MySqlService.log.Info("购买的是金币");
                        IsSuccess = BuyJin(goods, num, uid, moneyType);
                        break;
                    //购买的是元宝
                    case 2:
                        MySqlService.log.Warn("购买的是元宝,接口错误：" + uid);
                        break;
                    //购买的是道具
                    case 3:
                        MySqlService.log.Info("购买的是道具");
                        IsSuccess = BuyProp(goods, num, uid, moneyType);
                        break;
                }
            }
            if (IsSuccess)
            {
                OperatorSuccess(responseData);
            }
            else
            {
                OperatorFail(responseData);
            }
        }

        private bool BuyYuanbao(Goods goods, int num, string uid)
        {
            bool IsSuccess = false;
            //需要的人民币
            int sumPrice = goods.price * num;
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
            //只能用人民币
            if (goods.money_type == 3)
            {
                MySqlService.log.Info("添加元宝");
                if (AddYuanbao(goods, userInfo, num))
                {
                    IsSuccess = true;
                }
            }

            return IsSuccess;
        }

        //购买金币
        private bool BuyJin(Goods goods, int num, string uid, int moneyType)
        {
            bool IsSuccess = false;
            int sumPrice = goods.price * num;
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);

            switch (moneyType)
            {
                //元宝购买
                case 2:
                    if (userInfo.YuanBao >= sumPrice)
                    {
                        userInfo.YuanBao -= sumPrice;
                        //先扣钱,添加金币
                        MySqlService.log.Info("先扣钱,添加金币");
                        string temp = "";
                        if (NHibernateHelper.userInfoManager.Update(userInfo) && AddJinbi(goods, userInfo, num, "元宝购买金币", out temp))
                        {
                            //记录玩家财富变化日志
                            int after = userInfo.YuanBao;
                            int change = -sumPrice;
                            StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.YUANBAO, "元宝购买金币", after - change, change, after);

                           
                            string s = string.Format("花费{0}元宝，购买了{1}个{2}", sumPrice, num, goods.goods_name);
                            LogUtil.Log(uid, MyCommon.OpType.BUYGOLD, s);
                            IsSuccess = true;
                        }
                    }
                    break;
                case 4:
                    sumPrice = goods.price2 * num;

                    if (userInfo.Medel >= sumPrice)
                    {
                        userInfo.Medel -= sumPrice;
                        //先扣钱,添加金币
                        MySqlService.log.Info("先扣徽章,添加金币");
                        string temp = "";
                        if (NHibernateHelper.userInfoManager.Update(userInfo) && AddJinbi(goods, userInfo, num, "徽章购买金币", out temp))
                        {
                            //记录玩家财富变化日志
                            int after = userInfo.Medel;
                            int change = -sumPrice;
                            StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.Medal, "徽章购买金币", after - change, change, after);
                            string s = string.Format("花费{0}徽章，购买了{1}个{2}", sumPrice, num, goods.goods_name);
                            LogUtil.Log(uid, MyCommon.OpType.BUYGOLD, s);
                            IsSuccess = true;
                        }
                    }
                    break;
            }


        
            return IsSuccess;
        }

        private bool AddJinbi(Goods goods, UserInfo userInfo, int num, string reason, out string temp)
        {
            string[] strings = goods.props.Split(':');
            int propId = Convert.ToInt32(strings[0]);
            int propNum = Convert.ToInt32(strings[1]);
            if (propId == 1)
            {
                temp = propNum * num + "";
                userInfo.Gold += propNum * num;
                if (NHibernateHelper.userInfoManager.Update(userInfo))
                {
                    //记录玩家财富变化日志
                    int after = userInfo.Gold;
                    int change = propNum * num;
                    StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.GOLD, reason, after - change, change, after);
                    return true;
                }
            }
            temp = null;
            return false;
        }

        private bool AddYuanbao(Goods goods, UserInfo userInfo, int num)
        {
            string[] strings = goods.props.Split(':');
            int propId = Convert.ToInt32(strings[0]);
            int propNum = Convert.ToInt32(strings[1]);
            if (propId == 2)
            {
                userInfo.YuanBao += propNum * num;
                if (NHibernateHelper.userInfoManager.Update(userInfo))
                {
                    return true;
                }
            }
            return false;
        }

        private bool BuyProp(Goods goods, int num, string uid, int moneyType)
        {
            bool IsSuccess = false;
            //需要付的价格
            int sumPrice = goods.price * num;
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
           
            switch (moneyType)
            {
                //金币付款
                case 1:
                    MySqlUtil.ConfigExpenseGold(uid, sumPrice);

                    if (userInfo.Gold >= sumPrice)
                    {
                        userInfo.Gold -= sumPrice;

                        //先扣钱,添加道具
                        if (NHibernateHelper.userInfoManager.Update(userInfo))
                        {
                            for (int i = 0; i < num; i++)
                            {
                                if (!AddProp(goods, uid))
                                {
                                    return false;
                                }
                            }
                            string s = string.Format("花费{0}金币，购买了{1}个{2}", sumPrice, num, goods.goods_name);
                            LogUtil.Log(uid, MyCommon.OpType.BUYPROP, s);
                            IsSuccess = true;
                        }
                    }
                    break;
                //元宝付款
                case 2:
                    if (userInfo.YuanBao >= sumPrice)
                    {
                        userInfo.YuanBao -= sumPrice;
                        //先扣钱,添加道具
                        if (NHibernateHelper.userInfoManager.Update(userInfo))
                        {
                            for (int i = 0; i < num; i++)
                            {
                                if (!AddProp(goods, uid))
                                {
                                    return false;
                                }
                            }
                            string s = string.Format("花费{0}元宝，购买了{1}个{2}", sumPrice, num, goods.goods_name);
                            LogUtil.Log(uid, MyCommon.OpType.BUYPROP, s);
                            IsSuccess = true;
                        }
                    }
                    break;
                //徽章付款
                case 4:
                    sumPrice = goods.price2 * num;

                    if (userInfo.Medel >= sumPrice)
                    {
                        userInfo.Medel -= sumPrice;
                        //先扣钱,添加道具
                        if (NHibernateHelper.userInfoManager.Update(userInfo))
                        {
                            for (int i = 0; i < num; i++)
                            {
                                if (!AddProp(goods, uid))
                                {
                                    return false;
                                }
                            }
                            string s = string.Format("花费{0}徽章，购买了{1}个{2}", sumPrice, num, goods.goods_name);
                            LogUtil.Log(uid, MyCommon.OpType.BUYPROP, s);
                            IsSuccess = true;
                        }
                    }

                    break;
            }
            return IsSuccess;
        }

        private bool AddProp(Goods goods, string uid)
        {
            List<string> list = new List<String>();
            CommonUtil.splitStr(goods.props, list, ';');
            for (int i = 0; i < list.Count; i++)
            {
                string[] strings = list[i].Split(':');
                int propId = Convert.ToInt32(strings[0]);
                int propNum = Convert.ToInt32(strings[1]);

                UserProp userProp = NHibernateHelper.userPropManager.GetUserProp(uid, propId);
                if (userProp == null)
                {
                    userProp = new UserProp()
                    {
                        Uid = uid,
                        PropId = propId,
                        PropNum = propNum
                    };
                    if (!NHibernateHelper.userPropManager.Add(userProp))
                    {
                        MySqlService.log.Warn("添加道具失败");
                        return false;
                    }
                }
                else
                {
                    userProp.PropNum += propNum;
                    if (!NHibernateHelper.userPropManager.Update(userProp))
                    {
                        MySqlService.log.Warn("更新道具失败");
                        return false;
                    }
                }
            }
            return true;
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