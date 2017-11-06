using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJCommon;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class DeleteEmailHandler : BaseHandler
    {
        private int connId;

        public DeleteEmailHandler()
        {
            Tag = Consts.Tag_DeleteMail;
        }

        public override string OnResponse(string data)
        {
            ReadEmailReq readEmailReq = null;
            try
            {
                readEmailReq = JsonConvert.DeserializeObject<ReadEmailReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = readEmailReq.tag;
            connId = readEmailReq.connId;
            string uid = readEmailReq.uid;
            int emailId = readEmailReq.email_id;
            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || emailId < 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add(MyCommon.EMAIL_ID, emailId);

            //删除邮件
            DeleteEmailSql(emailId, uid, _responseData);
            return _responseData.ToString();
        }

        private void DeleteEmailSql(int emailId, string uid, JObject responseData)
        {
            UserEmail userEmail = MySqlService.userEmailManager.GetEmail(emailId, uid);
            if (userEmail == null)
            {
                MySqlService.log.Warn("该邮件不存在");
                OperatorFail(responseData);
            }
            else
            {
                //已读
                if (userEmail.State == 1)
                {
                    if (MySqlService.userEmailManager.Delete(userEmail))
                    {
                        OperatorSuccess(responseData);
                        MySqlService.log.Warn("删除邮件成功:" + uid);
                    }
                    else
                    {
                        MySqlService.log.Warn("删除邮件失败");
                        OperatorFail(responseData);
                    }
                }
                else
                {
                    MySqlService.log.Warn("该邮件未读");
                    OperatorFail(responseData);
                }
            }
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