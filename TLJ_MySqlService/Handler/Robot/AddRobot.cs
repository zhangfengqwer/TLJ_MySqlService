using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

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
                    Platform = 0,
                    Username = "ai"+i,
                    Userpassword = "",
                    IsRobot = 1
                };
                MySqlService.userManager.Add(user);
            }
        }
    }
}