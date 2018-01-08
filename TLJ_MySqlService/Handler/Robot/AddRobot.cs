using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    public class AddRobot
    {

        public static void AddRobotSql()
        {
            for (int i = 0; i < 50; i++)
            {
                User user = new User()
                {
                    Uid = UidUtil.createUID(),
                    ThirdId = "",
                    ChannelName = "javgame",
                    Username = "ai"+i,
                    Userpassword = "",
                    IsRobot = 1
                };
                NHibernateHelper.userManager.Add(user);
            }
        }
    }
}