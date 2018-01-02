using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_ChangeHead)]
    class ChangeUserHeadHandler : BaseHandler
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
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            var head = defaultReq.head;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || head < 1)
            {
                MySqlService.log.Warn($"字段有空:{data}");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.UID, uid);
            responseData.Add(MyCommon.CONNID, connId);


            ChangeUserHeadSQL(uid, head, responseData);
            return responseData.ToString();
        }

        private void ChangeUserHeadSQL(string uid, int head, JObject responseData)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn($"传入的uid未注册,{uid}");
            }
            else
            {
                UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);

                //用户信息表中没有用户信息
                if (userInfo != null)
                {
                    userInfo.Head = head;
                    NHibernateHelper.userInfoManager.Update(userInfo);
                    OperatorSuccess(responseData);
                }
                else
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn($"userinfo表没有该用户,{uid}");
                }
            }
        }

        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}