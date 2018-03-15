using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_SendMail_Admin)]
    class SendMailManagerHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            AddEmailReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<AddEmailReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string _tag = defaultReq.tag;
            int _connId = defaultReq.connId;
            string uid = defaultReq.uid;
            bool isToAll = defaultReq.isToAll;
            string title = defaultReq.title;
            string content = defaultReq.content;
            string reward = defaultReq.reward;


            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);
            _responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);

            if (isToAll)
            {
                ICollection<User> collection = MySqlManager<User>.Instance.GetAll();
                foreach (var user in collection)
                {
                    SendEmailUtil.SendEmail(user.Uid, title, content, reward);
                }
            }
            else
            {
                SendEmailUtil.SendEmail(uid, title, content, reward);
            }

           
            return _responseData.ToString();
        }
    }
}