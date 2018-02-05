using System;
using NhInterSqlServer.Model;
using NHibernate;
using NHibernate.Cfg;

namespace NhInterSqlServer
{
    public class NHibernaMsServerteHelper
    {
      
        private static ISessionFactory _sessionFactory;
        private static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
            {
                 var configuration = new Configuration();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                configuration.Configure(baseDirectory + "hibernate_sqlserver.cfg.xml");
                 configuration.AddAssembly("NhInterSqlServer");
                 _sessionFactory = configuration.BuildSessionFactory();
            }
                return _sessionFactory;
        }

        public static ISession OpenSession()
        {
            return GetSessionFactory().OpenSession();
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserSource GetById(int id)
        {
            using (var Session = NHibernaMsServerteHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.Get<UserSource>(id);
                    transaction.Commit();
                    return t;
                }
            }
        }
    }
}
