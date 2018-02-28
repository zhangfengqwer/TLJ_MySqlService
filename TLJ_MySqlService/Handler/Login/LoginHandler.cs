using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_Login)]
    class LoginHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            LoginReq login = null;
            try
            {
                login = JsonConvert.DeserializeObject<LoginReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误：" + e);
                return null;
            }

            string _tag = login.tag;
            int _connId = login.connId;
            string _username = login.account;
            string _userpassword = login.password;
            string ip = login.ip;
            string apkVersion = login.apkVersion;
            int passwordtype = login.passwordtype;
            string channelname = login.channelname;

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(_username) ||
                string.IsNullOrWhiteSpace(_userpassword))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add("passwordtype", passwordtype);

            if (_connId != 0)
            {
                _responseData.Add(MyCommon.CONNID, _connId);
            }

            User _user = new User() {Username = _username, Userpassword = _userpassword};
            LoginSQL(_user, passwordtype, channelname, ip, apkVersion, _responseData);
            return _responseData.ToString();
        }

        //登录 数据库操作
        private void LoginSQL(User user, int passwordtype, string channeName, string ip, string versionName, JObject responseData)
        {
            User loginUser = null;
            switch (passwordtype)
            {
                case 1:
                    loginUser = NHibernateHelper.userManager.VerifyLogin(user.Username,
                        CommonUtil.CheckPsw(user.Userpassword));
                    MySqlService.log.Info($"用户登陆");
                    break;
                //公众号登陆
                case 2:
                    loginUser = NHibernateHelper.userManager.VerifySecondLogin(user.Username,
                        CommonUtil.CheckPsw(user.Userpassword));
                    MySqlService.log.Info($"公众号二级密码登陆");
                    break;
                //游戏客户端登陆
                case 3:
                    loginUser = NHibernateHelper.userManager.VerifySecondLogin(user.Username,
                        CommonUtil.CheckPsw(user.Userpassword));
                    MySqlService.log.Info($"客户端二级密码登陆");
                    break;
            }

            if (loginUser != null)
            {
                if (passwordtype == 2)
                {
                    var userConfig = NHibernateHelper.commonConfigManager.GetByUid(loginUser.Uid);
                    if (userConfig == null)
                    {
                        userConfig = ModelFactory.CreateConfig(loginUser.Uid);
                    }

                    if (userConfig.wechat_login_gift == 0)
                    {
                        SendEmailUtil.SendEmail(loginUser.Uid, "神秘礼包", "您已成功关注微信公众号，并已绑定游戏账号，恭喜你获得一下奖励",
                            "110:2;1:1888");
                        userConfig.wechat_login_gift = 1;
                        NHibernateHelper.commonConfigManager.Update(userConfig);
                        LogUtil.Log(loginUser.Uid, MyCommon.OpType.WECHAT_LOGIN_GIFT, $"获得神秘礼包：110:2;1:1888");
                    }
                }
                OperatorSuccess(loginUser, channeName, ip, versionName, responseData);
            }
            else
            {
                User name = NHibernateHelper.userManager.GetByName(user.Username);
                if (name == null)
                {
                    responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_AccountNoExist);
                }
                else
                {
                    responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_PasswordError);
                }
            }
        }

        private void OperatorSuccess(User user, string channelName, string ip, string versionName, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);

            //更新下用户的任务
            MySqlUtil.UpdateUserTask(user.Uid);

            StatisticsHelper.StatisticsLogin(user.Uid);

            StatictisLogUtil.Login(user.Uid, user.Username, ip, channelName, versionName, MyCommon.OpType.Login);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}