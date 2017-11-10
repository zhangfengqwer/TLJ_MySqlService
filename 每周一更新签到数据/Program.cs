using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zfstu;
using TLJ_MySqlService;
using TLJ_MySqlService.Handler;
using TLJ_MySqlService.Utils;
using Zfstu.Model;

namespace UpdateSignMonday
{
    class Program
    {

        static void Main(string[] args)
        {
            new Thread(() =>
            {
                UpdateRechargeDayly();
            }).Start();
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

        //每天更新充值限额
        private static void UpdateRechargeDayly()
        {
            var sql = "update common_config set recharge_phonefee_amount = '0'";
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