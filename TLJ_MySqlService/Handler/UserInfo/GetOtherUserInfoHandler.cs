using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UserInfo_Game)]
    class GetOtherUserInfoHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            OtherUserInfoReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<OtherUserInfoReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            int isClientReq = defaultReq.isClientReq;

            if (string.IsNullOrWhiteSpace(Tag)
                || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
//            JObject responseData = new JObject();
//            responseData.Add(MyCommon.TAG, Tag);
//            responseData.Add(MyCommon.CONNID, connId);
//            responseData.Add(MyCommon.UID, uid);
//            responseData.Add("isClientReq", isClientReq);

            UserInfo_Game userInfoGame = new UserInfo_Game()
            {
                connId = connId,
                tag = Tag,
                uid = uid,
                isClientReq = isClientReq,
            };

            GetOtherUserInfoSql(uid, userInfoGame);

           

            return JsonConvert.SerializeObject(userInfoGame);
        }

        private void GetOtherUserInfoSql(string uid, UserInfo_Game userInfo_Game)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);
            if (user == null)
            {
                OperatorFail(userInfo_Game);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
                UserGame userGame = NHibernateHelper.userGameManager.GetByUid(uid);

                //用户信息表中没有用户信息
                if (userInfo == null)
                {
                    //注册用户数据
                    userInfo = GetUserInfoHandler.AddUserInfo(user.Uid, user.Username);

                    userGame = GetUserInfoHandler.AddUserGame(user.Uid);

                    if (NHibernateHelper.userInfoManager.Add(userInfo) &&
                        NHibernateHelper.userGameManager.Add(userGame))
                    {
                        OperatorSuccess(userInfo, userGame, userInfo_Game);
                    }
                    else
                    {
                        OperatorFail(userInfo_Game);
                        MySqlService.log.Warn("添加用户信息失败");
                    }
                }
                else
                {
                    OperatorSuccess(userInfo, userGame, userInfo_Game);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(UserInfo userInfo, UserGame userGame, UserInfo_Game userInfo_Game)
        {
            userInfo_Game.code = (int)Consts.Code.Code_OK;
            userInfo_Game.name = userInfo.NickName;

            var vipLevel = VipUtil.GetVipLevel(userInfo.RechargeVip);
            userInfo_Game.vipLevel = vipLevel;
            userInfo_Game.gold = userInfo.Gold;
            userInfo_Game.head = userInfo.Head;
            userInfo_Game.gameData = new UserInfo_Game.Gamedata();
            userInfo_Game.gameData.allGameCount = userGame.AllGameCount;
            userInfo_Game.gameData.winCount = userGame.WinCount;
            userInfo_Game.gameData.runCount = userGame.RunCount;
            userInfo_Game.gameData.meiliZhi = userGame.MeiliZhi;
          
            //用户buff
            List<UserInfo_Game.UserBuff> userBuffJsonObjects = new List<UserInfo_Game.UserBuff>();
            List<UserProp> userProps = NHibernateHelper.userPropManager.GetListByUid(userInfo.Uid);
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
            userInfo_Game.BuffData = userBuffJsonObjects;
        }

        //数据库操作失败
        private void OperatorFail(UserInfo_Game userInfo_Game)
        {
            userInfo_Game.code = (int) Consts.Code.Code_CommonFail;
        }
    }
}