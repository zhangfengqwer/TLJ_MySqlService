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
    class GetUserInfoHandler : BaseHandler
    {
        private static MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();
        private static MySqlManager<UserGame> userGameManager = new MySqlManager<UserGame>();
        private static MySqlManager<User> userManager = new MySqlManager<User>();

        public GetUserInfoHandler()
        {
            tag = Consts.Tag_UserInfo;
        }
       
        public override string OnResponse(string data)
        {
            DefaultReqData defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<DefaultReqData>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(Tag) || ConnId == 0
                || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            GetUserInfoSql(Uid,_responseData);
            return _responseData.ToString();
        }

        private void GetUserInfoSql(string uid, JObject responseData)
        {
            User user = userManager.GetByUid(uid);
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserInfo userInfo = userInfoManager.GetByUid(uid);
                UserGame userGame = userGameManager.GetByUid(uid);

                //用户信息表中没有用户信息
                if (userInfo == null)
                {
                    //注册用户数据
                    userInfo = new UserInfo()
                    {
                        Uid = user.Uid,
                        NickName = user.Uid,
                        Phone = "110",
                        Gold = 3000,
                        YuanBao = 0,
                        PlatForm = 0
                    };
                    userGame = new UserGame()
                    {
                        Uid = user.Uid,
                        AllGameCount = 0,
                        MeiliZhi =0,
                        RunCount = 0,
                        WinCount = 0
                    };


                    if (userInfoManager.Add(userInfo) && userGameManager.Add(userGame))
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
            UserGameJsonObject userGameJsonObject = new UserGameJsonObject(userGame.AllGameCount, userGame.WinCount, userGame.RunCount,
                userGame.MeiliZhi);
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add(MyCommon.NAME, userInfo.NickName);
            responseData.Add(MyCommon.PHONE, userInfo.Phone);
            responseData.Add(MyCommon.GOLD, userInfo.Gold);
            responseData.Add(MyCommon.YUANBAO, userInfo.YuanBao);
            responseData.Add(MyCommon.GAMEDATA, JsonConvert.SerializeObject(userGameJsonObject));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
