using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Manager;
using NhInterMySQL.Model;
using NhInterSqlServer.Model;
using System;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_OldPlayerBind)]
    class BindOldPlayerHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            OldPlayerReq oldPlayerReq = null;
            try
            {
                oldPlayerReq = JsonConvert.DeserializeObject<OldPlayerReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + data + "\n" + e);
                return null;
            }

            string oldUid = oldPlayerReq.old_uid;
            string uid = oldPlayerReq.uid;
            string from = oldPlayerReq.from;
            string machineId = oldPlayerReq.machine_id;
            int connId = oldPlayerReq.connId;

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(oldUid) || string.IsNullOrWhiteSpace(machineId))
            {
                MySqlService.log.Warn($"字段有空:{data}");
                return null;
            }

            JObject response = new JObject();
            response.Add(MyCommon.CONNID, connId);
            response.Add(MyCommon.TAG, Consts.Tag_OldPlayerBind);
            int i;
            try
            {
                i = int.Parse(oldUid);
            }
            catch (Exception e)
            {
                OperatorFail(response, Consts.Code.Code_OldPlayerUidIsNotExist);
                return response.ToString();
            }

            //先查询machineid
            Log_bind_oldplayer logBindOldplayer = MySqlManager<Log_bind_oldplayer>.Instance.GetOldPlayerByMacId(machineId);

            if (logBindOldplayer != null)
            {
                OperatorFail(response, Consts.Code.Code_ThePhoneIsBindOldUid);
                return response.ToString();
            }

            UserSource userSource = NhInterSqlServer.NHiMsServerteHelper.GetById(i);

            if (userSource == null)
            {
                OperatorFail(response, Consts.Code.Code_OldPlayerUidIsNotExist);
            }
            else
            {
                Log_bind_oldplayer bindOldplayer = MySqlManager<Log_bind_oldplayer>.Instance.GetByUid(uid);

                if (bindOldplayer == null)
                {
                    bindOldplayer = new Log_bind_oldplayer
                    {
                        Uid = uid,
                        old_uid = oldUid,
                        channel_name = from,
                        machine_id = machineId
                    };
                    if (MySqlManager<Log_bind_oldplayer>.Instance.Add(bindOldplayer))
                    {
                        SendEmailUtil.SendEmail(uid, "老用户特权奖励", "恭喜您已成功绑定老游戏UID，请领取您的奖励", "111:1;110:2;1:6666;107:5");
                        OperatorSuccess(response);
                    }
                    else
                    {
                        OperatorFail(response, Consts.Code.Code_TheOldUidIsUsed);
                    }
                }
                else
                {
                    OperatorFail(response, Consts.Code.Code_TheUidIsBind);
                }
            }

            return response.ToString();
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData, Consts.Code code)
        {
            responseData.Add(MyCommon.CODE, (int) code);
        }
    }
}