using Newtonsoft.Json;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TLJ_MySqlService.Model;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    class GetEmailHandler : BaseHandler
    {
       
        public GetEmailHandler()
        {
            Tag = Consts.Tag_GetMail;
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

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            UserEmailReq _userEmailReq = new UserEmailReq();
            _userEmailReq.mailData = new List<mailData>();
            _userEmailReq.tag = base.Tag;
            _userEmailReq.connId = connId;
            //查询
            GetUserEmailSql(uid, _userEmailReq);
            return JsonConvert.SerializeObject(_userEmailReq); ;
        }

        private void GetUserEmailSql(string uid, UserEmailReq _userEmailReq)
        {
            try
            {
                List<UserEmail> userEmailList = NHibernateHelper.userEmailManager.GetListByUid(uid).Take(50).ToList();
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
                    _userEmailReq.mailData.Add(mailData);
                }

                _userEmailReq.code = (int) Consts.Code.Code_OK;

            }
            catch (Exception e)
            {
                _userEmailReq.code = (int)Consts.Code.Code_CommonFail;
                MySqlService.log.Warn("获取邮箱数据失败:"+ e);
            }
        }
    }
}
