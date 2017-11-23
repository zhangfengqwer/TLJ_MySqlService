using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    class GetOtherUserInfoHandler : BaseHandler
    {
        public GetOtherUserInfoHandler()
        {
            Tag = Consts.Tag_UserInfo_Game;
        }

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
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            responseData.Add(MyCommon.UID, uid);
            responseData.Add("isClientReq", isClientReq);


            GetOtherUserInfoSql(uid, responseData);
            return responseData.ToString();
        }

        private void GetOtherUserInfoSql(string uid, JObject responseData)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);
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
                    //注册用户数据
                    userInfo = GetUserInfoHandler.AddUserInfo(user.Uid, user.Username);
                   
                    userGame = new UserGame()
                    {
                        Uid = user.Uid,
                        AllGameCount = 0,
                        MeiliZhi = 0,
                        RunCount = 0,
                        WinCount = 0
                    };


                    if (NHibernateHelper.userInfoManager.Add(userInfo) && NHibernateHelper.userGameManager.Add(userGame))
                    {
                        OperatorSuccess(userInfo, userGame, responseData);
                    }
                    else
                    {
                        OperatorFail(responseData);
                        MySqlService.log.Warn("添加用户信息失败");
                    }
                }
                else
                {
                    OperatorSuccess(userInfo, userGame, responseData);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(UserInfo userInfo, UserGame userGame, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.NAME, userInfo.NickName);
            responseData.Add(MyCommon.GOLD, userInfo.Gold);
            responseData.Add(MyCommon.HEAD, userInfo.Head);
            JObject gameData = new JObject();
            gameData.Add("allGameCount", userGame.AllGameCount);
            gameData.Add("winCount", userGame.WinCount);
            gameData.Add("runCount", userGame.RunCount);
            gameData.Add("meiliZhi", userGame.MeiliZhi);

            responseData.Add(MyCommon.GAMEDATA, gameData);
            //用户buff
            List<UserBuffJsonObject> userBuffJsonObjects = new List<UserBuffJsonObject>();
            List<UserProp> userProps = NHibernateHelper.userPropManager.GetListByUid(userInfo.Uid);
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

            responseData.Add("BuffData", JsonConvert.SerializeObject(userBuffJsonObjects));

        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}