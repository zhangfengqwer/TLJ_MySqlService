using NHibernate;
using NHibernate.Cfg;

namespace Zfstu
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
            {
                 var configuration = new Configuration();    
                 configuration.Configure();
                 configuration.AddAssembly("Zfstu");
                 _sessionFactory = configuration.BuildSessionFactory();
            }
                return _sessionFactory;
        }

        public static ISession OpenSession()
        {
            return GetSessionFactory().OpenSession();
        }
    }
}
