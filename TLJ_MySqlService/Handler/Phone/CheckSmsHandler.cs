using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using TLJCommon;
using TLJ_MySqlService.Model;
using TLJ_MySqlService.Utils;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class CheckSmsHandler : BaseHandler
    {
        
        public CheckSmsHandler()
        {
            tag = Consts.Tag_CheckSMS;
        }

        public override string OnResponse(string data)
        {
            CheckSmsReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<CheckSmsReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;
            string uid = defaultReqData.uid;
            string phoneNum = defaultReqData.phoneNum;
            string verfityCode = defaultReqData.verfityCode;

            if (string.IsNullOrWhiteSpace(Tag) || connId == 0
                || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(phoneNum) 
                || string.IsNullOrWhiteSpace(verfityCode))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);

            try
            {
                uid = uid.Substring(1, uid.Length - 1);
                string checkSms = HttpUtil.CheckSms(uid, phoneNum, verfityCode);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(checkSms);
                XmlNodeList nodeList = xmlDoc.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    string nodeValue = node.InnerText;
                    if ("string".Equals(node.Name))
                    {
                        MySqlService.log.Info(nodeValue);

                        JObject result = JObject.Parse(nodeValue);
                        var ResultCode =(int)result.GetValue("ResultCode");
                        if (ResultCode == 1)
                        {
                            uid = "6" + uid;
                            UserInfo userInfo = MySqlService.userInfoManager.GetByUid(uid);
                            if (userInfo == null)
                            {
                                MySqlService.log.Warn("uid未注册");
                                responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                            }
                            else
                            {
                                userInfo.Phone = phoneNum;
                                if (MySqlService.userInfoManager.Update(userInfo))
                                {
                                    responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
                                }
                                else
                                {
                                    MySqlService.log.Warn("更新用户数据库失败");
                                    responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                                }
                            }
                        }
                        else
                        {
                            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("发送信息失败:"+ e);
                responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
            }
            return responseData.ToString();
        }
    }
}