using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    class ThirdLoginHandler : BaseHandler
    {
       
        public ThirdLoginHandler()
        {
            Tag = Consts.Tag_Third_Login;
        }
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
            int platform = login.platform;
            if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(third_id)
                || string.IsNullOrWhiteSpace(nickname) || platform == 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, tag);
            responseData.Add(MyCommon.CONNID, connId);
            ThirdLoginSQL(third_id, nickname, platform, responseData);
            return responseData.ToString();
        }

        private void ThirdLoginSQL(string thirdId, string nickname, int platform, JObject responseData)
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
                    Platform = platform,
                    ThirdId = thirdId,
                    Secondpassword = "",
                    Uid = uid,
                    IsRobot = 0
                };

                var userEmail = new UserEmail()
                {
                    Uid = uid,
                    Title = "新用户奖励",
                    Content = "欢迎来到疯狂升级，为您送上1000金币，快去对战吧!",
                    Reward = "1:1000",
                    State = 0,
                    CreateTime = DateTime.Now,
                };

                Random random = new Random();

                //注册用户数据 并 注册新手邮箱
                if (NHibernateHelper.userManager.Add(user) && NHibernateHelper.userEmailManager.Add(userEmail))
                {
                    OperatorSuccess(user, responseData);
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < 10; i++)
                    {
                        int next = random.Next(1, 100);
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
                    }
                    else
                    {
                        MySqlService.log.Warn("第三方注册失败 user.Username:" + user.Username + "\nuser.Uid" + user.Uid);
                        OperatorFail(responseData);
                    }
                    
                }
            }
            else
            {
                OperatorSuccess(user, responseData);
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
