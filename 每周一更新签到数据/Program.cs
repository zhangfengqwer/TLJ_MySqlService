using System;
using System.Threading;
using Zfstu;

namespace UpdateSignMonday
{
    class Program
    {
        static void Main(string[] args)
        {
            new Thread(UpdateSignDays).Start();
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