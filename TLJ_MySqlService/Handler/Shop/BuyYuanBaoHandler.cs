using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class BuyYuanBaoHandler : BaseHandler
    {
        public BuyYuanBaoHandler()
        {
            Tag = Consts.Tag_BuyYuanBao;
        }

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

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || num == 0 || price == 0||
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

            BuyYuanBaoSql(goodId, num, uid, price, orderid, _responseData);
            return _responseData.ToString();
        }

        private void BuyYuanBaoSql(int goodId, int num, string uid, float price, string orderid, JObject responseData)
        {
            Goods goods = MySqlService.goodsManager.GetGoods(goodId);
            bool IsSuccess = false;

            if (goods != null)
            {
                IsSuccess = BuyYuanbao(goods, num, price, orderid, uid);
            }
            if (IsSuccess)
            {
                OperatorSuccess(responseData);
            }
            else
            {
                string msg = $"购买元宝失败，uid: {uid},goodid: {goodId},num: {num},price：{price},orderid: {orderid}";
                MySqlService.log.Warn(msg);
                LogUtil.Log(uid, MyCommon.OpType.BUYYUANBAO, msg);
                OperatorFail(responseData);
            }
        }

        private bool BuyYuanbao(Goods goods, int num, float price, string orderid, string uid)
        {
            bool IsSuccess = false;
            //需要的人民币
            int sumPrice = goods.price * num;

//            if (sumPrice != price)
//            {
//                MySqlService.log.Warn("价格不一致：" + sumPrice + "-" + price);
//                return false;
//            }

            UserInfo userInfo = MySqlService.userInfoManager.GetByUid(uid);
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
                userInfo.RechargeVip += goods.price * num;
                if (MySqlService.userInfoManager.Update(userInfo))
                {
                    var format = string.Format("花费了{0}元，购买了{1}元宝,订单号：{2}", price, propNum * num, orderid);
                    LogUtil.Log(userInfo.Uid, MyCommon.OpType.BUYYUANBAO, format);
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