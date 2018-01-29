using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Text.RegularExpressions;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_Third_Login)]
    class ThirdLoginHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            ThirdLoginReq login = null;
            try
            {
                login = JsonConvert.DeserializeObject<ThirdLoginReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string tag = login.tag;
            int connId = login.connId;
            string third_id = login.third_id;
            string nickname = login.nickname;
            string channelname = login.channelname;
            string ip = login.ip;
            if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(third_id)
                || string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(channelname) || "null".Equals(third_id))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, tag);
            responseData.Add(MyCommon.CONNID, connId);

            if (nickname.Length > 10)
            {
                nickname = nickname.Remove(5, nickname.Length - 10);
            }

            nickname = Regex.Replace(nickname, @"\p{Cs}", "");

            ThirdLoginSQL(third_id, nickname, channelname, ip, responseData);
            return responseData.ToString();
        }

        private void ThirdLoginSQL(string thirdId, string nickname, string channelname, string ip, JObject responseData)
        {

            //通过第三方查询用户
            User user = NHibernateHelper.userManager.GetUserByTid(thirdId);
            if (user == null)
            {
                string uid = UidUtil.createUID();
                user = new User()
                {
                    Username = nickname,
                    Userpassword = "",
                    ChannelName = channelname,
                    ThirdId = thirdId,
                    Secondpassword = "",
                    Uid = uid,
                    IsRobot = 0
                };

                Random random = new Random();

                //注册用户数据 并 注册新手邮箱
                if (NHibernateHelper.userManager.Add(user))
                {
                    OperatorSuccess(user, responseData);
                    StatictisLogUtil.Login(uid, user.Username, ip, channelname, "1.0.43", MyCommon.OpType.Register);
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < 10; i++)
                    {
                        int next = random.Next(1, 100);
                        user.Username.Remove(user.Username.Length - 1);
                        user.Username += next;
                        if (NHibernateHelper.userManager.Add(user))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        MySqlService.log.Info("第三方重复注册成功 user.Username:" + user.Username + "\nuser.Uid" + user.Uid);
                        OperatorSuccess(user, responseData);

                        //第三方注册
                        StatisticsHelper.StatisticsRegister(user.Uid);
                        StatictisLogUtil.Login(uid, user.Username, ip, channelname, "1.0.43", MyCommon.OpType.Register);
                    }
                    else
                    {
                        MySqlService.log.Warn("第三方注册失败 user.Username:" + user.Username + "\nuser.Uid" + user.Uid);
                        OperatorFail(responseData);
                    }
                }

                SendEmailUtil.SendEmail(uid, "新用户奖励", "欢迎来到疯狂升级，为您送上1000金币，快去对战吧!", "1:1000");
//                SendEmailUtil.SendEmail(uid, "“疯狂升级”新手指引",
//                    @"欢迎来到疯狂升级，本游戏有多种玩法供您选择，更有比赛场可以获取丰厚大奖噢！详细规则可在“关于-游戏规则”中查看，祝您游戏愉快~",
//                    "");
//                SendEmailUtil.SendEmail(uid, "“疯狂升级”游戏福利",
//                    @"每日登陆可领取签到奖励，通过游戏可获得抽奖机会，完成任务以达成成就，比赛场中获得胜利有丰厚大礼，更多精彩内容等你来玩噢~",
//                    "");
            }
            else
            {
                //第三方登陆
                OperatorSuccess(user, responseData);
                StatisticsHelper.StatisticsLogin(user.Uid);
            }
        }


        //数据库操作成功
        private void OperatorSuccess(User user, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);

            MySqlUtil.UpdateUserTask(user.Uid);
            ProgressTaskHandler.ProgressTaskSql(208, user.Uid);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}