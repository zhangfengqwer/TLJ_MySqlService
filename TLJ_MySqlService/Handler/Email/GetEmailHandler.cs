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
    class GetEmailHandler : BaseHandler
    {
       
        public GetEmailHandler()
        {
            tag = Consts.Tag_GetMail;
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
            UserEmailData userEmailData = new UserEmailData();
            userEmailData.mailData = new List<mailData>();
            userEmailData.tag = tag;
            userEmailData.connId = connId;
            //查询
            GetUserEmailSql(uid, userEmailData);
            return JsonConvert.SerializeObject(userEmailData); ;
        }

        private void GetUserEmailSql(string uid, UserEmailData userEmailData)
        {
            try
            {
                List<UserEmail> userEmailList = MySqlService.userEmailManager.GetListByUid(uid);
                mailData mailData;
                foreach (var userEmail in userEmailList)
                {
                    mailData = new mailData();
                    mailData.content = userEmail.Content;
                    mailData.title = userEmail.Title;
                    mailData.state = userEmail.State;
                    mailData.reward = userEmail.Reward;
                    mailData.email_id = userEmail.EmailId;
                    mailData.time = userEmail.CreateTime.ToLongDateString();
                    userEmailData.mailData.Add(mailData);
                }
                userEmailData.code = (int) Consts.Code.Code_OK;

            }
            catch (Exception e)
            {
                userEmailData.code = (int)Consts.Code.Code_CommonFail;
                MySqlService.log.Warn("获取邮箱数据失败:"+ e);
            }
        }
    }
}
