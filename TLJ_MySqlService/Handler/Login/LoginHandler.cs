using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJCommon;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class LoginHandler : BaseHandler
    {
       
        public LoginHandler()
        {
            Tag = Consts.Tag_Login;
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
            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(_username) ||  string.IsNullOrWhiteSpace(_userpassword))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData; _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);

            if (_connId != 0)
            {
                _responseData.Add(MyCommon.CONNID, _connId);
            }
         
            User _user = new User() { Username = _username, Userpassword = _userpassword };
            LoginSQL(_user, _responseData);
            return _responseData.ToString();

        }

        //登录 数据库操作
        private  void LoginSQL(User user, JObject responseData)
        {
            User loginUser = MySqlService.userManager.VerifyLogin(user.Username, user.Userpassword);
            if (loginUser != null)
            {
                OperatorSuccess(loginUser, responseData);
            }
            else
            {
                User name = MySqlService.userManager.GetByName(user.Username);
                if (name == null)
                {
                    responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_AccountNoExist);
                }
                else
                {
                    responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_PasswordError);
                }
               
            }
        }

        //数据库操作成功
        private  void OperatorSuccess(User user, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);
        }

        //数据库操作失败
        private  void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
