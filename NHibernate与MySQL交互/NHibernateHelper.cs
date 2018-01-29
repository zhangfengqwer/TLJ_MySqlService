using NhInterMySQL.Manager;
using NhInterMySQL.Model;
using NHibernate;
using NHibernate.Cfg;

namespace NhInterMySQL
{
    public class NHibernateHelper
    {
        public static MySqlManager<UserTask> userTaskManager = new MySqlManager<UserTask>();
        public static MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();
        public static MySqlManager<UserGame> userGameManager = new MySqlManager<UserGame>();
        public static MySqlManager<User> userManager = new MySqlManager<User>();
        public static MySqlManager<UserProp> userPropManager = new MySqlManager<UserProp>();
        public static MySqlManager<UserRealName> userRealNameManager = new MySqlManager<UserRealName>();
        public static MySqlManager<UserNotice> userNoticeManager = new MySqlManager<UserNotice>();
        public static MySqlManager<UserEmail> userEmailManager = new MySqlManager<UserEmail>();
        public static MySqlManager<Goods> goodsManager = new MySqlManager<Goods>();
        public static MySqlManager<Notice> noticeManager = new MySqlManager<Notice>();
        public static MySqlManager<Sign> signManager = new MySqlManager<Sign>();
        public static MySqlManager<PVPGameRoom> PVPGameRoomManager = new MySqlManager<PVPGameRoom>();
        public static MySqlManager<Task> taskManager = new MySqlManager<Task>();
        public static MySqlManager<SignConfig> signConfigManager = new MySqlManager<SignConfig>();
        public static MySqlManager<Prop> propManager = new MySqlManager<Prop>();
        public static MySqlManager<MyLog> logManager = new MySqlManager<MyLog>();
        public static MySqlManager<CommonConfig> commonConfigManager = new MySqlManager<CommonConfig>();
        public static MySqlManager<TurnTable> turnTableManager = new MySqlManager<TurnTable>();
        public static MySqlManager<UserRecharge> userRechargeManager = new MySqlManager<UserRecharge>();
        public static MySqlManager<Statistics> statisticsManager = new MySqlManager<Statistics>();
        public static MySqlManager<Log_Login> LogLoginManager = new MySqlManager<Log_Login>();
        public static MySqlManager<Log_Recharge> LogRechargeManager = new MySqlManager<Log_Recharge>();
        public static MySqlManager<Log_Online_Player> LogOnlinePlayerManager = new MySqlManager<Log_Online_Player>();
        public static MySqlManager<Log_Change_Wealth> LogChangeWealthManager = new MySqlManager<Log_Change_Wealth>();


        private static ISessionFactory _sessionFactory;
        private static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
            {
                 var configuration = new Configuration();    
                 configuration.Configure();
                 configuration.AddAssembly("NhInterMySQL");
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
