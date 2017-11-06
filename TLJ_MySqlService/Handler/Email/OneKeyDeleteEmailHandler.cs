using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TLJCommon;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class OneKeyDeleteEmailHandler : BaseHandler
    {
     

        public OneKeyDeleteEmailHandler()
        {
            Tag = Consts.Tag_OneKeyDeleteMail;
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

            //删除全部邮件
            OneKeyDeleteEmailSql(uid,_responseData);
            return _responseData.ToString() ;
        }

        private void OneKeyDeleteEmailSql(string uid, JObject responseData)
        {
            List<UserEmail> userEmails = MySqlService.userEmailManager.GetListByUid(uid);

            if (userEmails == null)
            {
                MySqlService.log.Warn("没有邮件可以删除");
                OperatorFail(responseData);
                return;
            }

            foreach (var email in userEmails)
            {
                if (email.State == 1)
                {
                    if (!MySqlService.userEmailManager.Delete(email))
                    {
                        MySqlService.log.Warn("删除邮件失败");
                        OperatorFail(responseData);
                        return;
                    }
                }
            }
            OperatorSuccess(responseData);
            MySqlService.log.Info("一键删除邮件成功");
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
