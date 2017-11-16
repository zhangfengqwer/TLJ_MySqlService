using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;
using TLJ_MySqlService.Model;
using TLJCommon;
using TLJ_MySqlService.Utils;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class UseHuaFeiHandler : BaseHandler
    {
        public UseHuaFeiHandler()
        {
            Tag = Consts.Tag_UseHuaFei;
        }

        public override string OnResponse(string data)
        {
            UseHuaFeiReq defaultReqData;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UseHuaFeiReq>(data);
            }
            catch (Exception)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;
            int propId = defaultReqData.prop_id;
            string phone = defaultReqData.phone;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid) || string.IsNullOrWhiteSpace(phone))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            CommonConfig commonConfig = MySqlService.commonConfigManager.GetByUid("6506476654");
            if (commonConfig?.recharge_phonefee_amount >= 100)
            {
                //当天充值金额已超过100元
                OperatorFail(_responseData);
                MySqlService.log.Warn($"{Uid}当天充值金额已超过100元,已充值：{commonConfig.recharge_phonefee_amount}");
                return _responseData.ToString();
            }

            UseHuaFeiSql(Uid, propId, phone, _responseData);
            return _responseData.ToString();
        }

        private void UseHuaFeiSql(string uid, int propId, string phone, JObject responseData)
        {
            UserProp userProp = MySqlService.userPropManager.GetUserProp(uid, propId);
            if (userProp == null || userProp.PropNum <= 0)
            {
                MySqlService.log.Warn($"没有该道具或者不能使用该道具:{uid}");
                OperatorFail(responseData);
            }
            else
            {
                userProp.PropNum--;
                bool isPhoneFee = true;
                switch (userProp.PropId)
                {
                    //1元话费
                    case 111:
                    //5元话费
                    case 112:
                        //测试环境不需要到账
//                        isPhoneFee = PhoneFeeRecharge(uid, propId, phone);
                        break;
                }
                if (isPhoneFee)
                {
                    if (MySqlService.userPropManager.Update(userProp))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        MySqlService.log.Warn("更新话费道具失败："+uid+" "+userProp.PropId);
                        OperatorFail(responseData);
                    }
                }
                else
                {
                    MySqlService.log.Warn("充值话费失败：" + uid + " " + propId);
                    OperatorFail(responseData);
                }
            }
        }   

        private bool PhoneFeeRecharge(string uid, int propId, string phone)
        {
            var prop = MySqlService.propManager.GetProp(propId);
            string amount = "0";
            if (propId == 111)
            {
                amount = "1";
            }
            else if (propId == 112)
            {
                amount = "5";
            }
            uid = uid.Substring(1, uid.Length - 1);
            string phoneFeeRecharge = HttpUtil.PhoneFeeRecharge(uid, prop.prop_name, amount, phone, propId + "", "1");
            uid = "6" + uid;

            MySqlService.log.Info(phoneFeeRecharge);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(phoneFeeRecharge);
            XmlNodeList nodeList = xmlDoc.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                string nodeValue = node.InnerText;
                if ("string".Equals(node.Name))
                {
                    MySqlService.log.Info(nodeValue);
                    JObject result = JObject.Parse(nodeValue);
                    var Code = (int) result.GetValue("Code");
                    var Message = (string) result.GetValue("Message");
                    var Orderid = (string) result.GetValue("Orderid");

                    MySqlService.log.Info("Code:" + Code + " Message:" + Message + " Orderid:" + Orderid);
                    string format = $"给{phone}充值了{amount}元话费,订单号：{Orderid}";

                    if (Code == 0)
                    {
                        CommonConfig commonConfig = MySqlService.commonConfigManager.GetByUid(uid);
                        if (commonConfig == null)
                        {
                            commonConfig = new CommonConfig()
                            {
                                Uid = uid,
                                recharge_phonefee_amount = 0,
                            };
                            MySqlService.commonConfigManager.Add(commonConfig);
                        }
                        //限制充值数量
                        commonConfig.recharge_phonefee_amount += Convert.ToInt32(amount);

                        MySqlService.commonConfigManager.Update(commonConfig);

                        LogUtil.Log(uid, MyCommon.OpType.RECHARGE_PHONEFEE, format);
                        return true;
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