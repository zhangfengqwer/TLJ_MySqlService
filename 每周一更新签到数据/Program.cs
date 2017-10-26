using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aop.Api;
using Aop.Api.Test;
using TLJ_MySqlService;
using TLJ_MySqlService.Utils;
using Zfstu;
using Zfstu.Manager;
using Zfstu.Model;

namespace UpdateSignMonday
{
    class Program
    {
        private static MySqlManager<PVPGameRoom> PVPGameRoomManager = new MySqlManager<PVPGameRoom>();
        static void Main(string[] args)
        {
//            new Thread(UpdateSignDays).Start();
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
    }
}