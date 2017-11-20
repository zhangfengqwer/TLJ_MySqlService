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
        private static HashSet<string> hashSet = new HashSet<string>();
        static void Main(string[] args)
        {
            bool a = hashSet.Add("123");
            Console.WriteLine(a);
            bool b = hashSet.Add("123");
            Console.WriteLine(b);

            bool contains = hashSet.Contains("123");
            Console.WriteLine(contains);
            //            List<TurnTable> turnTables = MySqlService.turnTableManager.GetAll().ToList();
            //            var signConfigs = MySqlService.signConfigManager.GetAll().ToList();

            //            List<UserInfo> userInfos = TLJ_MySqlService.MySqlService.userInfoManager.GetAll().ToList();
            //
            //            foreach (var userInfo in userInfos)
            //            {
            //                userInfo.freeCount = 3;
            //                userInfo.huizhangCount = 3;
            //                userInfo.luckyValue = 0;
            //                TLJ_MySqlService.MySqlService.userInfoManager.Update(userInfo);
            //            }

            //            GetProbabilityReward();
            //            var next = new Random().Next(0, 2);
            //            Console.WriteLine(next);
            //            float j = 1;
            //            int i = 1;
            //            Console.WriteLine(i == j);
            Console.ReadKey();
        }

        private static int GetProbabilityReward()
        {
            var probability1 = 32.6 * 100;
            var probability2 = 10.15 * 100;
            var probability3 = 5.89 * 100;
            var probability4 = 1 * 100;
            var probability5 = 25.49 * 100;
            var probability6 = 6 * 100;
            var probability7 = 9.6 * 100;
            var probability8 = 6.9 * 100;
            var probability9 = 2.32 * 100;
            var probability10 = 0.05 * 100;
            var doubles = new List<double>();

            doubles.Add(probability1);
            doubles.Add(probability2);
            doubles.Add(probability3);
            doubles.Add(probability4);
            doubles.Add(probability5);
            doubles.Add(probability6);
            doubles.Add(probability7);
            doubles.Add(probability8);
            doubles.Add(probability9);
            doubles.Add(probability10);

            var list = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                double temp = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    temp += doubles[j];
                }
                list.Add(temp);
            }

            foreach (var VARIABLE in list)
            {
                Console.WriteLine(VARIABLE);
            }

            int next = new Random().Next(1, 10001);
            int num = 0;
            if (next <= list[0]) num = 1;
            else if (next <= list[1]) num = 2;
            else if (next <= list[2]) num = 3;
            else if (next <= list[3]) num = 4;
            else if (next <= list[4]) num = 5;
            else if (next <= list[5]) num = 6;
            else if (next <= list[6]) num = 7;
            else if (next <= list[7]) num = 8;
            else if (next <= list[8]) num = 9;
            else if (next <= list[9]) num = 10;
            Console.WriteLine(next);
            Console.WriteLine(num);
            return num;
        }
    }
}
