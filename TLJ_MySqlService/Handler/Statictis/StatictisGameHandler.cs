using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GameStatistics)]
    class StatictisGameHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            Log_Game logGame = null;
            try
            {
                logGame = JsonConvert.DeserializeObject<Log_Game>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }

            NHibernateHelper.LogGameManager.Add(logGame);

            return null;
        }
    }
}