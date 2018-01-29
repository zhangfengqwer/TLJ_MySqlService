using Newtonsoft.Json;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_OnlineStatistics)]
    class StatictisOnlineHandler : BaseHandler
    {
        private int i = 1;

        public override string OnResponse(string data)
        {
            OnlinePlayerReq logOnlinePlayer = null;
            try
            {
                logOnlinePlayer = JsonConvert.DeserializeObject<OnlinePlayerReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }

            string uid = logOnlinePlayer.uid;
            int type = logOnlinePlayer.type;
            int roomId = logOnlinePlayer.room_id;
            bool isAi = logOnlinePlayer.isAI;
            string gameroomtype = logOnlinePlayer.gameroomtype;

            if (type != 3)
            {
                if (string.IsNullOrWhiteSpace(uid)|| string.IsNullOrWhiteSpace(gameroomtype) )
                {
                    MySqlService.log.Warn("字段有空:" + data);
                    return null;
                }
            }
           
            switch (type)
            {
                case 1:
                    int ai = isAi ? 1 : 0;
//                    List<Log_Login> logLogins = NHibernateHelper.LogLoginManager.GetListByUid(uid);
//                    if (logLogins.Count == 0)
//                    {
//                        MySqlService.log.Warn("未登录:" + data);
//                        return null;
//                    }
//                    Log_Login logLogin = logLogins[logLogins.Count - 1];

                    StatictisLogUtil.Online(uid, "", "", "", gameroomtype, roomId, ai);
                    break;
                case 2:
                    Log_Online_Player onlinePlayer = NHibernateHelper.LogOnlinePlayerManager.GetByUid(uid);
                    if (onlinePlayer != null)
                    {
                        NHibernateHelper.LogOnlinePlayerManager.Delete(onlinePlayer);
                    }
                    break;
                case 3:
                    var logOnlinePlayers = NHibernateHelper.LogOnlinePlayerManager.GetAll();
                    foreach (var player in logOnlinePlayers)
                    {
                        NHibernateHelper.LogOnlinePlayerManager.Delete(player);
                    }
                    break;
                default:
                    MySqlService.log.Warn("type越界:" + data);
                    break;
            }


            return null;
        }
    }
}