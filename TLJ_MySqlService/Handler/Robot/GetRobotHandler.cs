using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetAIList)]
    class GetRobotHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            GetAIReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<GetAIReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReq.tag;
            string account = defaultReq.account;
            string password = defaultReq.password;

            if (!MySqlService.AdminAccount.Equals(account) || !MySqlService.AdminPassWord.Equals(password))
            {
                MySqlService.log.Warn("账号错误");
                return null;
            }
            if (string.IsNullOrWhiteSpace(Tag))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);

            GetRobotUidSql(_responseData);
            return _responseData.ToString();
        }

        private void GetRobotUidSql(JObject responseData)
        {
            List<User> aiList = NHibernateHelper.userManager.GetAIList() as List<User>;
            List<AIJsonObject> aiJsonObjects = new List<AIJsonObject>();
            if (aiList == null) return;
            foreach (var ai in aiList)
            {
                var aiJsonObject = new AIJsonObject();
                aiJsonObject.uid = ai.Uid;
                aiJsonObjects.Add(aiJsonObject);
            }
            MySqlService.log.Info("AI数量:" + aiJsonObjects.Count);
            OperatorSuccess(aiJsonObjects, responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(List<AIJsonObject> aiJsonObjects, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("aiList", JsonConvert.SerializeObject(aiJsonObjects));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}