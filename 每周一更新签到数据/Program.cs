using System;
using System.Linq;
using System.Threading;
using Zfstu;
using TLJ_MySqlService;
using TLJ_MySqlService.Handler;

namespace UpdateSignMonday
{
    class Program
    {

        static void Main(string[] args)
        {
            new Thread(() =>
            {
                UpdateSignDays();
                UpdateCommonData();
            }).Start();
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