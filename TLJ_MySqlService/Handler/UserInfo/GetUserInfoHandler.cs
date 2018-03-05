using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UserInfo)]
    class GetUserInfoHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            DefaultReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<DefaultReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag) 
                || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            GetUserInfoSql(uid, responseData);
            return responseData.ToString();
        }

        private void GetUserInfoSql(string uid, JObject responseData)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);
            List<UserInfo_Game.UserBuff> userBuffJsonObjects = new List<UserInfo_Game.UserBuff>();
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
                UserGame userGame = NHibernateHelper.userGameManager.GetByUid(uid);

                //用户信息表中没有用户信息
                if (userInfo == null)
                {
                    userInfo = AddUserInfo(user.Uid,user.Username);

                    userGame = AddUserGame(user.Uid);

                    if (NHibernateHelper.userInfoManager.Add(userInfo) && NHibernateHelper.userGameManager.Add(userGame))
                    {
                        OperatorSuccess(userInfo, userGame, userBuffJsonObjects, false,responseData);
                    }
                    else
                    {
                        OperatorFail(responseData);
                        MySqlService.log.Warn("添加用户信息失败");
                    }
                }
                else
                {
                    //得到buff数据
                    List<UserProp> userProps = NHibernateHelper.userPropManager.GetListByUid(uid);
                    if (userProps != null)
                    {
                        for (int i = 0; i < userProps.Count; i++)
                        {
                            if (userProps[i].BuffNum > 0)
                            {
                                UserInfo_Game.UserBuff userBuffJsonObject = new UserInfo_Game.UserBuff()
                                {
                                    prop_id = userProps[i].PropId,
                                    buff_num = userProps[i].BuffNum
                                };
                                userBuffJsonObjects.Add(userBuffJsonObject);
                            }
                        }
                    }

                    //是否实名
                    UserRealName userRealName = NHibernateHelper.userRealNameManager.GetByUid(uid);
                    OperatorSuccess(userInfo, userGame, userBuffJsonObjects, userRealName != null, responseData);
                }
            }
        }

        //添加用户数据
        public static UserInfo AddUserInfo(string uid,string nickName)
        {

            UserInfo userInfo = new UserInfo()
            {
                Uid = uid,
                NickName = nickName,
                Head = new Random().Next(1, 17),
                Phone = "",
                Gold = 5000,
                YuanBao = 0,
                RechargeVip = 0,
                Medel = 0,
                freeCount = 0,
                huizhangCount = 3,
                luckyValue = 0,
            };
            return userInfo;
        }
        
        //添加用户game数据
        public static UserGame AddUserGame(string uid)
        {
            UserGame  userGame = new UserGame()
            {
                Uid = uid,
                AllGameCount = 0,
                MeiliZhi = 0,
                RunCount = 0,
                WinCount = 0,
                XianxianCDHigh = 0,
                XianxianCDMiddle = 0,
                XianxianCDPrimary = 0,
                XianxianJDHigh = 0,
                XianxianJDMiddle = 0,
                XianxianJDPrimary = 0
            };
            return userGame;
        }

        //数据库操作成功
        private void OperatorSuccess(UserInfo userInfo, UserGame userGame, List<UserInfo_Game.UserBuff> userProps, bool isRealName, JObject responseData)
        {
            UserGameJsonObject userGameJsonObject = new UserGameJsonObject(userGame.AllGameCount, userGame.WinCount, userGame.RunCount, userGame.MeiliZhi,
                userGame.XianxianJDPrimary,userGame.XianxianJDMiddle,userGame.XianxianJDHigh,
                userGame.XianxianCDPrimary,userGame.XianxianCDMiddle,userGame.XianxianCDHigh );
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.NAME, userInfo.NickName);
            responseData.Add(MyCommon.PHONE, userInfo.Phone);
            responseData.Add(MyCommon.GOLD, userInfo.Gold);
            responseData.Add("medal", userInfo.Medel);
            responseData.Add("isRealName", isRealName);
            responseData.Add("recharge_vip", userInfo.RechargeVip);
            responseData.Add(MyCommon.YUANBAO, userInfo.YuanBao);
            responseData.Add(MyCommon.HEAD, userInfo.Head);
            responseData.Add(MyCommon.GAMEDATA, JsonConvert.SerializeObject(userGameJsonObject));
            responseData.Add("BuffData", JsonConvert.SerializeObject(userProps));

            //得到转盘次数
            var turnTableJsonObject = new TurnTableJsonObject();
            turnTableJsonObject.freeCount = userInfo.freeCount;
            turnTableJsonObject.huizhangCount = userInfo.huizhangCount;
            turnTableJsonObject.luckyValue = userInfo.luckyValue;
            responseData.Add("turntableData", JsonConvert.SerializeObject(turnTableJsonObject));

            //获取用户二级密码
            User user = NHibernateHelper.userManager.GetByUid(userInfo.Uid);
            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(user.Secondpassword))
                {
                    responseData.Add("isSetSecondPsw", false);
                }
                else
                {
                    responseData.Add("isSetSecondPsw", true);
                }
            }
            else
            {
                MySqlService.log.Warn($"获取用户失败：{userInfo.Uid}");
            }

            //获取用户充值次数
            var userRecharges = NHibernateHelper.userRechargeManager.GetListByUid(userInfo.Uid);

            responseData.Add("userRecharge", JsonConvert.SerializeObject(userRecharges));

            CommonConfig config = ModelFactory.CreateConfig(userInfo.Uid);
            if (config.first_recharge_gift == 0)
            {
                responseData.Add("hasShouChong", false);
            }
            else
            {
                responseData.Add("hasShouChong", true);
            }

            if (string.IsNullOrWhiteSpace(userInfo.ExtendCode))
            {
                while (true)
                {
                    userInfo.ExtendCode = RandomCharHelper.GetRandomNum(6);
                    if(NHibernateHelper.userInfoManager.Update(userInfo))
                    {
                        break;
                    }
                }
            }

            responseData.Add("myTuiGuangCode", userInfo.ExtendCode);



        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}