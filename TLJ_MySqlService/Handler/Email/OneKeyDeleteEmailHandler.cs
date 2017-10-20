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
    class OneKeyDeleteEmailHandler : BaseHandler
    {
        private static MySqlManager<UserEmail> userEmailManager = new MySqlManager<UserEmail>();
        private UserEmailData userEmailData;

        public OneKeyDeleteEmailHandler()
        {
            tag = Consts.Tag_OneKeyDeleteMail;
        }

        public override string OnResponse(string data)
        {
            DefaultReqData defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<DefaultReqData>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;
            string uid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(Tag) || connId == 0 || string.IsNullOrWhiteSpace(uid))
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
            List<UserEmail> userEmails = userEmailManager.GetListByUid(uid);

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
                    if (!userEmailManager.Delete(email))
                    {
                        MySqlService.log.Warn("删除邮件失败");
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
