using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class RecordUserGameDataHandler : BaseHandler
    {
        public RecordUserGameDataHandler()
        {
            Tag = Consts.Tag_RecordUserGameData;
        }

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

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }


            //得到pvp数据
            RecordUserGameDataSql(uid, actionType);
            return null;
        }

        private void RecordUserGameDataSql(string uid, int actionType)
        {
            UserGame userGame = MySqlService.userGameManager.GetByUid(uid);
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
                if (MySqlService.userGameManager.Update(userGame))
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