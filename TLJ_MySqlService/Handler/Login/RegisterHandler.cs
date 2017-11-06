using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class RegisterHandler : BaseHandler
    {
      
        public RegisterHandler()
        {
            Tag = Consts.Tag_QuickRegister;
        }

        public override string OnResponse(string data)
        {
            LoginReq login = null;
            try
            {
                login = JsonConvert.DeserializeObject<LoginReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string _tag = login.tag;
            int _connId = login.connId;
            string _username = login.account;
            string _userpassword = login.password;
            if (string.IsNullOrWhiteSpace(_tag)  
                || string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_userpassword))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData; _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);
            User _user = new User() {Username = _username, Userpassword = _userpassword};
            RegisterSQL(_user, _responseData);
            return _responseData.ToString();
        }

        //注册 数据库操作
        private void RegisterSQL(User user, JObject responseData)
        {
            User userByName = MySqlService.userManager.GetByName(user.Username);
            if (userByName != null)
            {
                OperatorFail(responseData);
            }
            else
            {
                string uid = UidUtil.createUID();
                user.Uid = uid;
                user.Platform = 0;
                user.ThirdId = "";
                user.IsRobot = 0;

                var userEmail = new UserEmail()
                {
                    Uid = uid,
                    Title = "新用户奖励",
                    Content = "欢迎来到疯狂升级，为您送上1000金币，快去对战吧!",
                    Reward = "1:1000",
                    State = 0,
                    CreateTime = DateTime.Now,
                };

                //注册用户数据 并 注册新手邮箱
                if (MySqlService.userManager.Add(user)&& MySqlService.userEmailManager.Add(userEmail))
                {
                    OperatorSuccess(user, responseData);
                }
                else
                {
                    OperatorFail(responseData);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(User user, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}