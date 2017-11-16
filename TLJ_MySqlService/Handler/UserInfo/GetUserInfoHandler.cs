using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class GetUserInfoHandler : BaseHandler
    {
        public GetUserInfoHandler()
        {
            Tag = Consts.Tag_UserInfo;
        }

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
            User user = MySqlService.userManager.GetByUid(uid);
            List<UserBuffJsonObject> userBuffJsonObjects = new List<UserBuffJsonObject>();
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserInfo userInfo = MySqlService.userInfoManager.GetByUid(uid);
                UserGame userGame = MySqlService.userGameManager.GetByUid(uid);

                //用户信息表中没有用户信息
                if (userInfo == null)
                {
                    userInfo = AddUserInfo(user.Uid,user.Username);

                    userGame = new UserGame()
                    {
                        Uid = user.Uid,
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


                    if (MySqlService.userInfoManager.Add(userInfo) && MySqlService.userGameManager.Add(userGame))
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
                    List<UserProp> userProps = MySqlService.userPropManager.GetListByUid(uid);
                    if (userProps != null)
                    {
                        for (int i = 0; i < userProps.Count; i++)
                        {
                            if (userProps[i].BuffNum > 0)
                            {
                                UserBuffJsonObject userBuffJsonObject = new UserBuffJsonObject()
                                {
                                    prop_id = userProps[i].PropId,
                                    buff_num = userProps[i].BuffNum
                                };
                                userBuffJsonObjects.Add(userBuffJsonObject);
                            }
                        }
                    }

                    //是否实名
                    UserRealName userRealName = MySqlService.userRealNameManager.GetByUid(uid);
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
                Head = new Random().Next(1, 16),
                Phone = "",
                Gold = 2000,
                YuanBao = 0,
                RechargeVip = 0,
                Medel = 0,
                freeCount = 0,
                huizhangCount = 3,
                luckyValue = 0
            };
            return userInfo;
        }

        //数据库操作成功
        private void OperatorSuccess(UserInfo userInfo, UserGame userGame, List<UserBuffJsonObject> userProps, bool isRealName, JObject responseData)
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
            User user = MySqlService.userManager.GetByUid(userInfo.Uid);
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
          
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}