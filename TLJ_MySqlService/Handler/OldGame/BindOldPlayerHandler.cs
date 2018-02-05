using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NhInterMySQL.Manager;
using NhInterSqlServer.Model;
using TLJ_MySqlService.JsonObject;
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
                MySqlService.log.Warn("传入的参数有误:" + data);
                return null;
            }

            string oldUid = oldPlayerReq.old_uid;
            string uid = oldPlayerReq.uid;
            string from = oldPlayerReq.from;
            int connId = oldPlayerReq.connId;

            JObject response = new JObject();
            response.Add(MyCommon.CONNID, connId);
            response.Add(MyCommon.TAG, Consts.Tag_OldPlayerBind);

            UserSource userSource = NhInterSqlServer.NHibernaMsServerteHelper.GetById(int.Parse(oldUid));

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
                        channel_name = from
                    };
                    if (MySqlManager<Log_bind_oldplayer>.Instance.Add(bindOldplayer))
                    {
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
            responseData.Add(MyCommon.CODE, (int)code);
        }
    }
}