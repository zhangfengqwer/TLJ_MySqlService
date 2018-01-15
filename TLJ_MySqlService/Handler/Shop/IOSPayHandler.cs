using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_IOS_Pay)]
    class IOSPayHandler : BaseHandler
    {
        private int i = 1;

        public override string OnResponse(string data)
        {
            DefaultReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<DefaultReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReq.tag;
            int ConnId = defaultReq.connId;
            string uid = defaultReq.uid;
            string reqData = defaultReq.data;
            string productId = defaultReq.productId;


            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid)
                || string.IsNullOrWhiteSpace(reqData) || string.IsNullOrWhiteSpace(productId))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Consts.Tag_BuyYuanBao);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add(MyCommon.UID, uid);

            Goods goods = NhInterMySQL.NHibernateHelper.goodsManager.GetGoods(int.Parse(productId));

            string response = null;
            if (VerifyReceipt(reqData) && goods != null)
            {
                BuyYuanBaoReq baoReq = new BuyYuanBaoReq()
                {
                    account = MySqlService.AdminAccount,
                    password = MySqlService.AdminPassWord,
                    connId = -1,
                    goods_id = int.Parse(productId),
                    goods_num = 1,
                    tag = Consts.Tag_BuyYuanBao,
                    uid = uid,
                    price = goods.price,
                    order_id = GetOrderId(),
                };
                response = new BuyYuanBaoHandler().OnResponse(JsonConvert.SerializeObject(baoReq));

                MySqlService.log.Warn($"处理返回：{response}");

                OperatorSuccess(_responseData);
            }
            else
            {
                OperatorFail(_responseData);
            }

            return response;
        }

       

        private string GetOrderId()
        {
            return DateTime.Now.ToString("yyMMddHHmmss" + ++i);
        }


        private bool VerifyReceipt(string reqData)
        {
            string sandboxurl = "https://sandbox.itunes.apple.com/verifyReceipt";
            string onlineurl = "https://buy.itunes.apple.com/verifyReceipt";

            string postHttp = HttpUtil.PostHttp(onlineurl, reqData);

            MySqlService.log.Info($"苹果正式返回数据：{postHttp}");

            JObject jObject = JObject.Parse(postHttp);
            if (jObject.TryGetValue("status", out var status))
            {
                if (0 == (int) status)
                {
                    return true;
                }else if (21007 == (int) status)
                {
                    string result = HttpUtil.PostHttp(sandboxurl, reqData);
                    MySqlService.log.Info($"苹果沙箱返回数据：{postHttp}");
                    JObject jo = JObject.Parse(result);
                    if (jo.TryGetValue("status", out var sts))
                    {
                        if (0 == (int) sts)
                        {
                            return true;
                        }
                    }
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