using System;
using System.Collections.Generic;
using NhInterSqlServer.Model;
using NHibernate;
using NHibernate.Cfg;
using System.Linq;
using NHibernate.Criterion;

namespace NhInterSqlServer
{
    public class NHiMsServerteHelper
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
            using (var Session = NHiMsServerteHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.Get<UserSource>(id);
                    transaction.Commit();
                    return t;
                }
            }
        }

        public static T GetById<T>(int id)
        {
            using (var Session = NHiMsServerteHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.Get<T>(id);
                    transaction.Commit();
                    return t;
                }
            }
        }

        public static List<T> GetAll<T>()
        {
            using (var Session = NHiMsServerteHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.CreateCriteria(typeof(T))
                        .Add(Restrictions.Gt("ModifyTime",DateTime.Parse("2017-01-01 00:00:00")))
                        .List<T>().ToList();
                    transaction.Commit();
                    return t;
                }
            }
        }
        public static List<T> GetAll2<T>()
        {
            using (var Session = NHiMsServerteHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.CreateCriteria(typeof(T))
                        .AddOrder(Order.Desc("ModifyTime"))
                        .List<T>().ToList();
                    transaction.Commit();
                    return t;
                }
            }
        }

    }
}