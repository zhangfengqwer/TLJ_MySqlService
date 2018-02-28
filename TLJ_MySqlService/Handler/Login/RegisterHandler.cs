using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_QuickRegister)]
    class RegisterHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string _tag = login.tag;
            int _connId = login.connId;
            string _username = login.account;
            string _userpassword = login.password;
            string channelname = login.channelname;
            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_userpassword) ||
                string.IsNullOrWhiteSpace(channelname))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData;
            _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);
            User _user = new User() {Username = _username, Userpassword = _userpassword};
            RegisterSQL(_user, channelname, _responseData);
            return _responseData.ToString();
        }

        //注册 数据库操作
        private void RegisterSQL(User user, string channelname, JObject responseData)
        {
            User userByName = NHibernateHelper.userManager.GetByName(user.Username);
            if (userByName != null)
            {
                OperatorFail(responseData, "用户已存在");
            }
            else
            {
                string uid = UidUtil.createUID();
                user.Uid = uid;
                user.ChannelName = channelname;
                user.ThirdId = "";
                user.Secondpassword = "";
                user.IsRobot = 0;
                user.Userpassword = CommonUtil.CheckPsw(user.Userpassword);

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
                if (NHibernateHelper.userManager.Add(user) && NHibernateHelper.userEmailManager.Add(userEmail))
                {
                    OperatorSuccess(user, responseData);

//                    SendEmailUtil.SendEmail(uid, "“疯狂升级”新手指引",
//                        @"欢迎来到疯狂升级，本游戏有多种玩法供您选择，更有比赛场可以获取丰厚大奖噢！详细规则可在“关于-游戏规则”中查看，祝您游戏愉快~",
//                        "");
//                    SendEmailUtil.SendEmail(uid, "“疯狂升级”游戏福利",
//                        @"每日登陆可领取签到奖励，通过游戏可获得抽奖机会，完成任务以达成成就，比赛场中获得胜利有丰厚大礼，更多精彩内容等你来玩噢~",
//                        "");
                }
                else
                {
                    OperatorFail(responseData, "用户注册失败");
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(User user, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);

            MySqlUtil.UpdateUserTask(user.Uid);
            ProgressTaskHandler.ProgressTaskSql(208, user.Uid);

            StatisticsHelper.StatisticsRegister(user.Uid);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}