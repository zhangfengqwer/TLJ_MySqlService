using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HPSocketCS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class LoginHandler : BaseHandler
    {
        private static MySqlManager<User> userManager = new MySqlManager<User>();
      
        public LoginHandler()
        {
            tag = Consts.Tag_Login;
        }
        public override string OnResponse(string data)
        {
            Login login = null;
            try
            {
                login = JsonConvert.DeserializeObject<Login>(data);
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
            if (string.IsNullOrWhiteSpace(_tag) || _connId == 0 
                || string.IsNullOrWhiteSpace(_username) ||  string.IsNullOrWhiteSpace(_userpassword))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData; _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);
            User _user = new User() { Username = _username, Userpassword = _userpassword };
            LoginSQL(_user, _responseData);
            return _responseData.ToString();

        }

        //登录 数据库操作
        private  void LoginSQL(User user, JObject responseData)
        {
            User loginUser = userManager.VerifyLogin(user.Username, user.Userpassword);
            if (loginUser != null)
            {
                OperatorSuccess(loginUser, responseData);
            }
            else
            {
                OperatorFail(responseData);
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
