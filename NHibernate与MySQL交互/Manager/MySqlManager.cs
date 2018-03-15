using System;
using System.Collections;
using System.Collections.Generic;
using NhInterMySQL.Model;
using NHibernate.Criterion;
using System.Linq;
using Task = NhInterMySQL.Model.Task;

namespace NhInterMySQL.Manager
{
    public class MySqlManager<T> : IManager<T>
    {
        private static readonly object Locker = new object();

        private string tableName = null;

        private static MySqlManager<T> instance;

        public static MySqlManager<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MySqlManager<T>();
                }

                return instance;
            }
        }

        public MySqlManager()
        {
            if (typeof(T) == typeof(User))
            {
                tableName = "Username";
            }
            else if (typeof(T) == typeof(UserInfo))
            {
                tableName = "NickName";
            }
            else
            {
                tableName = "Uid";
            }
        }

        /// <summary>
        ///  增
        /// </summary>
        /// <param name="t"></param>
        /// <returns>是否操作成功</returns>
        public bool Add(T t)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    try
                    {
                        lock (Locker)
                        {
                            Session.Save(t);
                            transaction.Commit();
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 删
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Delete(T t)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    try
                    {
                        Session.Delete(t);
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update(T t)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    try
                    {
                        lock (Locker)
                        {
                            Session.Update(t);
                            transaction.Commit();
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 增改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool AddOrUpdate(T t)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    try
                    {
                        lock (Locker)
                        {
                            Session.SaveOrUpdate(t);
                            transaction.Commit();
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(int id)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    var t = Session.Get<T>(id);
                    transaction.Commit();
                    return t;
                }
            }
        }


        /// <summary>
        /// 根据名字查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetByName(string name)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var criteria = Session.CreateCriteria(typeof(T));
                var t = criteria.Add(Restrictions.Eq(tableName, name)).UniqueResult<T>();
                return t;
            }
        }

        /// <summary>
        /// 根据Uid查询
        /// </summary>
        /// <param uid="uid"></param>
        /// <returns></returns>
        public T GetByUid(string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var criteria = Session.CreateCriteria(typeof(T));
                var t = criteria.Add(Restrictions.Eq("Uid", uid)).UniqueResult<T>();
                return t;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<T> GetListByUid(string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var criteria = Session.CreateCriteria(typeof(T));
                var t = criteria.Add(Restrictions.Eq("Uid", uid)).List<T>();
                return t as List<T>;
            }
        }

        /// <summary>
        /// 根据Uid查询
        /// </summary>
        public T GetGoods(int goodId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var criteria = Session.CreateCriteria(typeof(T));
                var t = criteria.Add(Restrictions.Eq("goods_id", goodId)).UniqueResult<T>();
                return t;
            }
        }

        /// <summary>
        /// 得到未读取邮件
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<UserEmail> GetUnReadByUid(string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var criteria = Session.CreateCriteria(typeof(UserEmail));
                var t = criteria.Add(Restrictions.Eq("Uid", uid))
                    .Add(Restrictions.Eq("State", 0))
                    .List<UserEmail>();
                return t as List<UserEmail>;
            }
        }


        /// <summary>
        /// 得到所有
        /// </summary>
        /// <returns></returns>
        public ICollection<T> GetAll()
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                IList<T> t = Session.CreateCriteria(typeof(T))
                    .List<T>();
                return t;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property">Gold;Medel</param>
        /// <returns></returns>
        public List<UserInfo> GetAllUserInfo()
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                List<UserInfo> t = (List<UserInfo>) Session.CreateCriteria(typeof(UserInfo))
                    .List<UserInfo>();
                return t;
            }
        }

        /// <summary>
        /// 登录验证 User表独自用
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User VerifyLogin(string username, string password)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var user = Session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("Username", username))
                    .Add(Restrictions.Eq("Userpassword", password))
                    .UniqueResult<User>();

                return user;
            }
        }

        /// <summary>
        /// 登录二级登录验证 User表独自用
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User VerifySecondLogin(string username, string secondPassword)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var user = Session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("Username", username))
                    .Add(Restrictions.Eq("Secondpassword", secondPassword))
                    .UniqueResult<User>();

                return user;
            }
        }

        public UserEmail GetEmail(int emailId, string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userEmail = Session.CreateCriteria(typeof(UserEmail))
                    .Add(Restrictions.Eq("EmailId", emailId))
                    .Add(Restrictions.Eq("Uid", uid))
                    .UniqueResult<UserEmail>();

                return userEmail;
            }
        }

        public UserProp GetUserProp(string uid, int propId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userProp = Session.CreateCriteria(typeof(UserProp))
                    .Add(Restrictions.Eq("PropId", propId))
                    .Add(Restrictions.Eq("Uid", uid))
                    .UniqueResult<UserProp>();

                return userProp;
            }
        }

        public UserNotice getUserNotice(string uid, int noticeId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userNotice = Session.CreateCriteria(typeof(UserNotice))
                    .Add(Restrictions.Eq("NoticeId", noticeId))
                    .Add(Restrictions.Eq("Uid", uid))
                    .UniqueResult<UserNotice>();

                return userNotice;
            }
        }

        public UserTask GetUserTask(string uid, int taskId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userTask = Session.CreateCriteria(typeof(UserTask))
                    .Add(Restrictions.Eq("task_id", taskId))
                    .Add(Restrictions.Eq("Uid", uid))
                    .UniqueResult<UserTask>();

                return userTask;
            }
        }

        public UserRecharge GetUserRecharge(string uid, int goodId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userRecharge = Session.CreateCriteria(typeof(UserRecharge))
                    .Add(Restrictions.Eq("goods_id", goodId))
                    .Add(Restrictions.Eq("Uid", uid))
                    .UniqueResult<UserRecharge>();
                return userRecharge;
            }
        }

        public Task GetTask(int taskId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var task = Session.CreateCriteria(typeof(Task))
                    .Add(Restrictions.Eq("task_id", taskId))
                    .UniqueResult<Task>();

                return task;
            }
        }

        /// <summary>
        /// 获得金币排行榜
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IList<UserInfo> GetGoldRank(int num)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                IList<UserInfo> userInfos = Session.QueryOver<UserInfo>()
                    .OrderBy(p => p.Gold).Desc
                    .OrderBy(p => p.NickName).Asc
                    .Take(num)
                    .List();
                return userInfos;
            }
        }

        /// <summary>
        /// 获得徽章排行榜
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IList<UserInfo> GetMedalRank(int num)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                IList<UserInfo> userInfos = Session.QueryOver<UserInfo>()
                    .OrderBy(p => p.Medel).Desc
                    .Take(num)
                    .List();
                return userInfos;
            }
        }

        public List<User> GetUserByTid(string thirdId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var user = Session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("ThirdId", thirdId))
                    .List<User>().ToList();
                return user;
            }
        }

        public IList<User> GetAIList()
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var user = Session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("IsRobot", 1))
                    .List<User>();
                return user;
            }
        }


        public Prop GetProp(int propId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var Prop = Session.CreateCriteria(typeof(Prop))
                    .Add(Restrictions.Eq("prop_id", propId))
                    .UniqueResult<Prop>();

                return Prop;
            }
        }

        public PVPGameRoom GetPVPRoom(string gameroomtype)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var Prop = Session.CreateCriteria(typeof(PVPGameRoom))
                    .Add(Restrictions.Eq("gameroomtype", gameroomtype))
                    .UniqueResult<PVPGameRoom>();

                return Prop;
            }
        }

        public Statistics GetStatisByHour(string currentHour)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                lock (Locker)
                {
                    var statistics = Session.CreateCriteria(typeof(Statistics))
                        .Add(Restrictions.Eq("time", currentHour))
                        .UniqueResult<Statistics>();

                    return statistics;
                }
            }
        }


        public Log_bind_oldplayer GetOldPlayerByMacId(string machineId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                lock (Locker)
                {
                    var statistics = Session.CreateCriteria(typeof(Log_bind_oldplayer))
                        .Add(Restrictions.Eq("machine_id", machineId))
                        .UniqueResult<Log_bind_oldplayer>();
                    return statistics;
                }
            }
        }

        public UserMonthSign GetUserMonthSign(string uid, string signYearMonth, string signDate)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userMonthSign = Session.CreateCriteria(typeof(UserMonthSign))
                    .Add(Restrictions.Eq("Uid", uid))
                    .Add(Restrictions.Eq("SignDate", signDate))
                    .Add(Restrictions.Eq("SignYearMonth", signYearMonth))
                    .UniqueResult<UserMonthSign>();
                return userMonthSign;
            }
        }

        public UserInfo GetUserInfoByCode(string code)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userInfo = Session.CreateCriteria(typeof(UserInfo))
                    .Add(Restrictions.Eq("ExtendCode", code))
                    .UniqueResult<UserInfo>();
                return userInfo;
            }
        }

        public IList<UserExtend> GetMyExtendDataByUid(string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var userExtends = Session.CreateCriteria(typeof(UserExtend))
                    .Add(Restrictions.Eq("extend_uid", uid))
                    .List<UserExtend>();
                return userExtends;
            }
        }

        public List<T> GetByPorpertyAndUid(string porperty, string value, string uid)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var t = Session.CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(porperty, value))
                    .Add(Restrictions.Eq("Uid", uid))
                    .List<T>();
                return (List<T>) t;
            }
        }

        public List<T> GetByPorperty(string porperty, string value)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var t = Session.CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(porperty, value))
                    .List<T>();
                return (List<T>) t;
            }
        }
        public List<T> GetByPorperty(string porperty1, string value1, string porperty2, string value2)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var t = Session.CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(porperty1, value1))
                    .Add(Restrictions.Eq(porperty2, value2))
                    .List<T>();
                return (List<T>) t;
            }
        }

        public List<UserExchange> GetMedalLimitMonth(string uid)
        {
            int nowYear = DateTime.Now.Year;
            int nowMonth = DateTime.Now.Month;
            string date;
            if (nowMonth < 10)
            {
                date = $"{nowYear}-0{nowMonth}%";
            }
            else
            {
                date = $"{nowYear}-{nowMonth}%";
            }

            var sql = $"select * from user_exchange  where uid = '{uid}'  and create_time like '{date}'";

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        List<UserExchange> list = session.CreateSQLQuery(sql).AddEntity(typeof(UserExchange))
                            .List<UserExchange>().ToList();
                        transaction.Commit();
                        return list;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return null;
        }
    }
}