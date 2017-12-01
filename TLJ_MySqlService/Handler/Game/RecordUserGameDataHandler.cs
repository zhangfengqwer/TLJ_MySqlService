using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_RecordUserGameData)]
    class RecordUserGameDataHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            RecordUserDataReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<RecordUserDataReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReq.tag;
            string uid = defaultReq.uid;
            int actionType = defaultReq.action_type;
            string room_type = defaultReq.room_type;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(room_type))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }


            //得到pvp数据
            RecordUserGameDataSql(uid, room_type, actionType);
            return null;
        }

        private void RecordUserGameDataSql( string uid, string room_type, int actionType)
        {
            UserGame userGame = NHibernateHelper.userGameManager.GetByUid(uid);
            if (userGame == null)
            {
                MySqlService.log.Warn("userGame表内没有改uid：" + uid);
            }
            else
            {
                switch (actionType)
                {
                    case (int) Consts.GameAction.GameAction_StartGame:
                        userGame.AllGameCount++;
                        userGame.DailyGameCount++;
                        //当天每玩一局，转盘次数加1;
                        if (userGame.DailyGameCount <= 3)
                        {
                            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
                            userInfo.freeCount++;
                            NHibernateHelper.userInfoManager.Update(userInfo);
                            MySqlService.log.Info($"{uid}-{userInfo.NickName}的转盘次数加1，当前为{userInfo.freeCount}");
                        }


                        if (room_type.Equals(Consts.GameRoomType_XiuXian_JingDian_ChuJi))
                        {
                            userGame.XianxianJDPrimary++;
                        }
                        else if (room_type.Equals(Consts.GameRoomType_XiuXian_ChaoDi_ZhongJi))
                        {
                            userGame.XianxianJDMiddle++;
                        }else if (room_type.Equals(Consts.GameRoomType_XiuXian_JingDian_GaoJi))
                        {
                            userGame.XianxianCDHigh++;
                        }
                        else if (room_type.Equals(Consts.GameRoomType_XiuXian_ChaoDi_ChuJi))
                        {
                            userGame.XianxianCDPrimary++;
                        }else if (room_type.Equals(Consts.GameRoomType_XiuXian_ChaoDi_ZhongJi))
                        {
                            userGame.XianxianCDMiddle++;
                        }else if (room_type.Equals(Consts.GameRoomType_XiuXian_ChaoDi_GaoJi))
                        {
                            userGame.XianxianCDHigh++;
                        }

                        break;
                    case (int) Consts.GameAction.GameAction_Win:
                        userGame.WinCount++;
                        break;
                    case (int) Consts.GameAction.GameAction_Run:
                        userGame.RunCount++;
                        break;
                    default:
                        MySqlService.log.Warn("没有该actionType:" + actionType);
                        break;
                }

                MySqlService.log.Info("RecordUserGameDataHandler-actionType:" + actionType+ "room_type:"+ room_type);
                if (NHibernateHelper.userGameManager.Update(userGame))
                {
                }
                else
                {
                    MySqlService.log.Warn("userGame表更新失败+" + actionType);
                }
            }
        }


        //数据库操作成功
        private void OperatorSuccess(List<PVPGameRoom> pvpGameRooms, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("room_list", JsonConvert.SerializeObject(pvpGameRooms));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}