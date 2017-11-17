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
            new Thread(() => { UpdateDaily(); }).Start();
        }

        private static void UpdateDaily()
        {
            UpdateTurnTableCount();
            UpdateRechargeDayly();
            UpdateDailyGameCount();
        }

        private static void UpdateWeekly()
        {
            UpdateSignDays();
            UpdateCommonData();
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

        //每天更新转盘次数
        private static void UpdateTurnTableCount()
        {
            string sql = "update user_info set huizhangCount = '3' ,freeCount = '0'";

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

            List<UserInfo> userInfos = MySqlService.userInfoManager.GetAll().ToList();

            foreach (var userinfo in userInfos)
            {
                int level = VipUtil.GetVipLevel(userinfo.RechargeVip);

                if (level >= 3)
                {
                    userinfo.freeCount = level - 2;
                    MySqlService.userInfoManager.Update(userinfo);
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

        //每天更新玩家一天的游戏局数
        private static void UpdateDailyGameCount()
        {
            var sql = "update user_game set daily_game_count = '0'";
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
            MySqlService.log.Info("更新每日签到配置和商城列表");
        }
    }
}