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
    [Handler(Consts.Tag_CheckWeChatRegister)]
    class CheckWeChatRegisterHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            DefaultReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<DefaultReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }

            string Tag = defaultReq.tag;
            string unionid = defaultReq.unionid;
            int connId = defaultReq.connId;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(unionid))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);

            List<User> users = MySqlManager<User>.Instance.GetUserByTid(unionid);
            string time = "";
            string uid = "";
            string extendCode = "";
            if (users.Count > 0)
            {
                var user = users[0];
                uid = user.Uid;
                time = user.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            responseData.Add("uid", uid);
            responseData.Add("time", time);
            responseData.Add("extendCode", time);
            return responseData.ToString();
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