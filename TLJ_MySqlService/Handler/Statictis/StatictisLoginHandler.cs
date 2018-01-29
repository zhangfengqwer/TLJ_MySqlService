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
    [Handler("LoginDataStatistics")]
    class StatictisLoginHandler : BaseHandler
    {
        private int i = 1;

        public override string OnResponse(string data)
        {
            StatictisLoginReq statictisLoginReq = null;
            try
            {
                statictisLoginReq = JsonConvert.DeserializeObject<StatictisLoginReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }

            string uid = statictisLoginReq.uid;
            string ip = statictisLoginReq.ip;
            string apkVersion = statictisLoginReq.apkVersion;
            string channelname = statictisLoginReq.channelname;
            int type = statictisLoginReq.type;

            if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(uid)
                || string.IsNullOrWhiteSpace(apkVersion) || string.IsNullOrWhiteSpace(channelname)|| type < 1)
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            User user = NHibernateHelper.userManager.GetByUid(uid);
            if (user == null)
            {
                MySqlService.log.Warn("uid未注册：" + data);
            }
            switch (type)
            {
                case 1:
                    StatictisLogUtil.Login(uid, user.Username, ip, channelname, apkVersion, MyCommon.OpType.Login);
                    break;
                case 2:
                    StatictisLogUtil.Login(uid, user.Username, ip, channelname, apkVersion, MyCommon.OpType.Register);
                    break;
            }
            return null;
        }
    }
}