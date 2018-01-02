using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJ_MySqlService.Model;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_SendSMS)]
    class SendSmsHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            SendSmsReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<SendSmsReq>(data);
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

            if (string.IsNullOrWhiteSpace(Tag) 
                || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(phoneNum))
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
                responseData.Add("result", HttpUtil.SendSms(uid, phoneNum));
                responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("发送信息失败:" + e);
                responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            }

            return responseData.ToString();
        }
    }
}