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
    class AddEmailHandler : BaseHandler
    {
        public AddEmailHandler()
        {
            Tag = Consts.Tag_SendMailToUser;
        }

        public override string OnResponse(string data)
        {
            AddEmailReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<AddEmailReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReq.tag;
            string Uid = defaultReq.uid;
            string account = defaultReq.account;
            string password = defaultReq.password;
            string title = defaultReq.title;
            string content = defaultReq.content;
            string reward = defaultReq.reward;

            if (!MySqlService.AdminAccount.Equals(account) || !MySqlService.AdminPassWord.Equals(password))
            {
                MySqlService.log.Warn("账号错误");
                return null;
            }

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid)
                || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) ||
                string.IsNullOrWhiteSpace(reward))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            //查询
            AddUserEmailSql(Uid, title, content, reward, _responseData);
            return _responseData.ToString();
        }

        private void AddUserEmailSql(string uid, string title, string content, string reward, JObject responseData)
        {
            var userEmail = new UserEmail()
            {
                Uid = uid,
                Title = title,
                Content = content,
                Reward = reward,
                State = 0,
                CreateTime = DateTime.Now,
            };
            if (MySqlService.userEmailManager.Add(userEmail))
            {
                OperatorSuccess(responseData);
                MySqlService.log.Info("添加用户邮箱成功:" + uid);
            }
            else
            {
                MySqlService.log.Warn("添加用户邮箱失败");
                OperatorFail(responseData);
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