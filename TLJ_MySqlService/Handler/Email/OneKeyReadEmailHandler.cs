using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    class OneKeyReadEmailHandler : BaseHandler
    {
      

        public OneKeyReadEmailHandler()
        {
            Tag = Consts.Tag_OneKeyReadMail;
        }

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
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag)  || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);

            //读取全部邮件
            OneKeyReadEmailSql(uid,_responseData);
            return _responseData.ToString() ;
        }

        private void OneKeyReadEmailSql(string uid, JObject responseData)
        {
            //得到未读取的邮件
            List<UserEmail> userEmails = NHibernateHelper.userEmailManager.GetListByUid(uid);

            if (userEmails == null)
            {
                MySqlService.log.Warn("没有未读取的邮件");
                OperatorFail(responseData);
                return;
            }

            foreach (var email in userEmails)
            {
                if (email.State == 0)
                {
                    email.State = 1;

                    if (!string.IsNullOrWhiteSpace(email.Reward))
                    {
                        bool addProp = MySqlUtil.AddProp(uid, email.Reward);
                        if (!addProp)
                        {
                            MySqlService.log.Warn("读邮件加道具失败：" + uid + " " + email.Reward);
                        }
                    }

                    if (!NHibernateHelper.userEmailManager.Update(email))
                    {
                        MySqlService.log.Warn("读取失败");
                        OperatorFail(responseData);
                        return;
                    }
                }
            }
            OperatorSuccess(responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
