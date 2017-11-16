using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLJ_MySqlService;
using TLJ_MySqlService.Handler;
using Zfstu.Model;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TurnTable> turnTables = MySqlService.turnTableManager.GetAll().ToList();
            var signConfigs = MySqlService.signConfigManager.GetAll().ToList();

            //            List<UserInfo> userInfos = TLJ_MySqlService.MySqlService.userInfoManager.GetAll().ToList();
            //
            //            foreach (var userInfo in userInfos)
            //            {
            //                userInfo.freeCount = 3;
            //                userInfo.huizhangCount = 3;
            //                userInfo.luckyValue = 0;
            //                TLJ_MySqlService.MySqlService.userInfoManager.Update(userInfo);
            //            }


            float j = 1f;
            int i = 1;
            Console.WriteLine(i == j);
            Console.ReadKey();
        }

       
    }
}
