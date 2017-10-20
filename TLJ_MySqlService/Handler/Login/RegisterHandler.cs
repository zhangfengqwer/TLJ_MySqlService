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
        private static MySqlManager<User> userManager = new MySqlManager<User>();
        private static MySqlManager<Sign> signManager = new MySqlManager<Sign>();
        private static MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();

        public RegisterHandler()
        {
            tag = Consts.Tag_QuickRegister;
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
            User userByName = userManager.GetByName(user.Username);
            if (userByName != null)
            {
                OperatorFail(responseData);
            }
            else
            {
              
                string uid = UidUtil.createUID();
                user.Uid = uid;
                //注册签到数据
                Sign sign = new Sign()
                {
                    Uid = uid,
                    SignWeekDays = 0,
                    UpdateTime = DateTime.Now
                };
                //注册用户数据
                UserInfo userInfo = new UserInfo()
                {
                    Uid = user.Uid,
                    NickName = user.Uid,
                    Phone = "110",
                    Gold = 3000,
                    YuanBao = 0,
                    PlatForm = 0
                };

                //注册用户 并 注册用户的签到数据
                if (userManager.Add(user) && signManager.Add(sign) && userInfoManager.Add(userInfo))
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