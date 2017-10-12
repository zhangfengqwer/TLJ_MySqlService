using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class DeleteEmailHandler : BaseHandler
    {
        private static MySqlManager<UserEmail> userEmailManager = new MySqlManager<UserEmail>();
        private int connId;

        public DeleteEmailHandler()
        {
            tag = Consts.Tag_DeleteMail;
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
            if (string.IsNullOrWhiteSpace(Tag) || connId == 0 || string.IsNullOrWhiteSpace(uid) || emailId < 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add(MyCommon.EMAIL_ID, emailId);

            //读取邮件
            DeleteEmailSql(emailId,uid,_responseData);
            return _responseData.ToString() ;
        }

        private void DeleteEmailSql(int emailId, string uid, JObject responseData)
        {
            UserEmail userEmail = userEmailManager.GetEmail(emailId, uid);
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
                    if (userEmailManager.Delete(userEmail))
                    {
                        OperatorSuccess(responseData);
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
        private void OperatorSuccess( JObject responseData)
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
