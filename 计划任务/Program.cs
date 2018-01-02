using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using NhInterMySQL;
using NhInterMySQL.Model;
using NLog;

namespace UpdateSignMonday
{
    class Program
    {
        public static Logger log = LogManager.GetLogger("MySqlService");

        static void Main(string[] args)
        {
            new Thread(() =>
            {
                UpdateDaily();
            }).Start();
        }

        private static void UpdateDaily()
        {
            UpdateTurnTableCount();
            UpdateCommonConfig();
            UpdateDailyGameCount();
            UpdateUserTask();
            UpdateFreeGold();
            log.Info("数据库每天更新啦");
        }

        private static void UpdateWeekly()
        {
            UpdateSignDays();
            UpdateCommonData();
            SendVipWeeklyReWard();
            log.Info("数据库每周一更新啦");
        }

        private static void UpdateFreeGold()
        {
            var sql = "update user_task set progress = '0',isover = '0' ";
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

        private static void UpdateUserTask()
        {
            var sql = "update user_task set progress = '0',isover = '0' ";
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
        /// <summary>
        /// 每周发放vip奖励
        /// </summary>
        private static void SendVipWeeklyReWard()
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "VipRewardData.json");
            string str = sr.ReadToEnd();
            sr.Close();
            List<VipData> vipDatas = JsonConvert.DeserializeObject<List<VipData>>(str);

            List<UserInfo> userInfos = NHibernateHelper.userInfoManager.GetAll().ToList();
            foreach (var userInfo in userInfos)
            {
                int vipLevel = VipUtil.GetVipLevel(userInfo.RechargeVip);
                if (vipLevel > 0)
                {
                    VipData vipData = vipDatas[vipLevel - 1];
                    var reward = $"1:{vipData.vipWeekly.goldNum};107:{vipData.vipWeekly.diamondNum}";
                    var sql = $"INSERT INTO user_email (uid, title, content,reward,state)" +
                              $" VALUES ('{userInfo.Uid}', 'vip每周奖励','vip每周奖励','{reward}','0')";

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
            }
        }

        private static List<User> GetRobot()
        {
            List<User> userGames = NHibernateHelper.userManager.GetAll().ToList();
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
        /// <summary>
        /// 每周更新签到的天数
        /// </summary>
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

            List<UserInfo> userInfos = NHibernateHelper.userInfoManager.GetAll().ToList();
            //vip要特殊处理
            foreach (var userinfo in userInfos)
            {
                int level = VipUtil.GetVipLevel(userinfo.RechargeVip);

                if (level < 3)
                {
                    userinfo.freeCount = 0;
                }
                else if (level <= 5)
                {
                    userinfo.freeCount = 1;
                    NHibernateHelper.userInfoManager.Update(userinfo);
                }
                else if (level <= 8)
                {
                    userinfo.freeCount = 2;
                    NHibernateHelper.userInfoManager.Update(userinfo);
                }
                else if (level <= 10)
                {
                    userinfo.freeCount = 3;
                    NHibernateHelper.userInfoManager.Update(userinfo);
                }
            }
        }

        //每天更新充值限额和每天花费金币数,每天发放3次免费
        private static void UpdateCommonConfig()
        {
            var sql = "update common_config set recharge_phonefee_amount = '0',expense_gold_daily = '0',free_gold_count = '3'";
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
            
        }
    }
}