using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Xml;
using NhInterMySQL.Manager;
using TLJ_MySqlService.Model;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UseHuaFei)]
    class UseHuaFeiHandler : BaseHandler
    {
        private static readonly object Locker = new object();

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
            int prop_num = defaultReqData.prop_num;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid) || string.IsNullOrWhiteSpace(phone) ||
                prop_num < 1)
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            if (prop_num % 5 != 0 && prop_num != 1)
            {
                OperatorFail(_responseData, $"道具数量不正确:{prop_num}");
                return _responseData.ToString();
            }
            else
            {
                MySqlService.log.Info($"兑换话费:{propId},{prop_num}");
            }

            //当天充值金额已超过100元
            CommonConfig commonConfig = NHibernateHelper.commonConfigManager.GetByUid(Uid);
            if (commonConfig?.recharge_phonefee_amount >= 100)
            {
                OperatorFail(_responseData, $"今日话费兑换额度已达上限");
                MySqlService.log.Error($"{Uid}当天充值金额已超过100元,已充值：{commonConfig.recharge_phonefee_amount}");
                return _responseData.ToString();
            }

            //当天每个手机30限额
            List<UserPhoneExchange> userPhoneExchanges = MySqlManager<UserPhoneExchange>.Instance.GetByCurrentDay(phone);
            int totalMoney = 0;
            foreach (var userPhoneExchange in userPhoneExchanges)
            {
                totalMoney += userPhoneExchange.Money;
            }

            if (totalMoney >= 30)
            {
                OperatorFail(_responseData, $"该手机充值已达限制");
                return _responseData.ToString();
            }

            lock (Locker)
            {
                UseHuaFeiSql(Uid, propId, phone, prop_num, _responseData);
            }

            return _responseData.ToString();
        }

        private void UseHuaFeiSql(string uid, int propId, string phone, int num, JObject responseData)
        {
            UserProp userProp = NHibernateHelper.userPropManager.GetUserProp(uid, propId);
            UserGame userGame = MySqlManager<UserGame>.Instance.GetByUid(uid);

            //限制话费使用
            if (userGame == null)
            {
                MySqlService.log.Warn($"没有该道具或者不能使用该道具:{uid},{propId},{num}");
                OperatorFail(responseData, "没有该道具或者不能使用该道具");
                return;
            }
            else
            {
                if (userGame.DailyGameCount < 1)
                {
                    MySqlService.log.Info($"{uid}:当天未完成一次对局");
                    OperatorFail(responseData, "充值话费失败:完成一次对局方可使用");
                    return;
                }
            }

            if (userProp == null || userProp.PropNum < num)
            {
                MySqlService.log.Warn($"没有该道具或者不能使用该道具:{uid},{propId},{num}");
                OperatorFail(responseData, "没有该道具或者不能使用该道具");
            }
            else
            {
                userProp.PropNum -= num;
                bool isPhoneFee = true;
                switch (userProp.PropId)
                {
                    //1元话费
                    case 111:
                    //5元话费
                    case 112:
                    //10元话费
                    case 113:
                        //测试环境不需要到账
                        if (!MySqlService.IsTest)
                        {
                            isPhoneFee = PhoneFeeRecharge(uid, propId, phone, num);
                        }

                        break;
                }

                if (isPhoneFee)
                {
                    if (NHibernateHelper.userPropManager.Update(userProp))
                    {
                        MySqlService.log.Info($"成功充值,还剩{userProp.BuffNum}");

                        //记录充值记录
                        int amount = 0;
                        if (propId == 111)
                        {
                            amount = 1 * num;
                        }
                        else if (propId == 112)
                        {
                            amount = 5;
                        }
                        else if (propId == 113)
                        {
                            amount = 10;
                        }

                        
                        UserPhoneExchange userPhoneExchange = new UserPhoneExchange()
                        {
                            Uid = uid,
                            Money = amount,
                            Phone = phone,
                            CreateTime = DateTime.Now
                        };
                        MySqlManager<UserPhoneExchange>.Instance.Add(userPhoneExchange);
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        MySqlService.log.Warn("更新话费道具失败：" + uid + " " + userProp.PropId);
                        OperatorFail(responseData, "更新话费道具失败");
                    }
                }
                else
                {
                    MySqlService.log.Warn($"充值话费失败,uid:{uid},propid:{propId},phone:{phone}");
                    OperatorFail(responseData, "充值话费失败");
                }
            }
        }

        private bool PhoneFeeRecharge(string uid, int propId, string phone, int num)
        {
            var prop = NHibernateHelper.propManager.GetProp(propId);
            string amount = "0";
            if (propId == 111)
            {
                amount = 1 * num + "";
            }
            else if (propId == 112)
            {
                amount = "5";
            }
            else if (propId == 113)
            {
                amount = "10";
            }
            else
            {
                return false;
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
                        CommonConfig commonConfig = NHibernateHelper.commonConfigManager.GetByUid(uid);
                        if (commonConfig == null)
                        {
                            commonConfig = ModelFactory.CreateConfig(uid);
                        }

                        //限制充值数量
                        commonConfig.recharge_phonefee_amount += Convert.ToInt32(amount);

                        NHibernateHelper.commonConfigManager.Update(commonConfig);

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
        private void OperatorFail(JObject responseData, string s)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", s);
        }
    }
}