using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zfstu;
using TLJ_MySqlService;
using TLJ_MySqlService.Handler;
using Zfstu.Model;

namespace UpdateSignMonday
{
    class Program
    {

        static void Main(string[] args)
        {
            IList<User> aiList = MySqlService.userManager.GetAIList();
            //            List<UserProp> userProps = MySqlService.userPropManager.GetAll().ToList();
            //
            //            for (int i = 0; i < userProps.Count; i++)
            //            {
            //                UserProp userProp =  userProps[i];
            //                if (userProp.PropId == 102 || userProp.PropId == 105)
            //                {
            //                    MySqlService.userPropManager.Delete(userProp);
            //                }
            //            }
//            MySqlService.ShopData = MySqlService.goodsManager.GetAll().ToList();
//            for (int i = 0; i < MySqlService.ShopData.Count; i++)
//            {
//                Goods shopData = MySqlService.ShopData[i];
//                if (shopData.goods_id == 1005)
//                {
//                    MySqlService.ShopData.Remove(shopData);
//                }
//                //            if (shopData.goods_id == 1002 || shopData.goods_id == 1005)
//                //            {
//                //                shopDataList.Remove(shopData);
//                //            }
//            }
            Console.ReadKey();

//            new Thread(() =>
//            {
//                UpdateSignDays();
//                UpdateCommonData();
//            }).Start();
        }

        private static List<User> GetRobot()
        {
            List<User> userGames = MySqlService.userManager.GetAll().ToList();
            int i = 0;
            foreach (User user in userGames)
            {
                if (user.IsRobot == 1)
                {
                    i++;
                    Console.WriteLine(user.Username);
                }
            }
            return userGames;
        }

        private static void UpdateSignDays()
        {
            var sql = "update sign set sign_week_days = '0' ";
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.CreateSQLQuery(sql).ExecuteUpdate();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        //更新每日签到配置和商城列表
        private static void UpdateCommonData()
        {
            MySqlService.ShopData = MySqlService.goodsManager.GetAll().ToList();
            MySqlService.SignConfigs = MySqlService.signConfigManager.GetAll().ToList();
        }
    }
}