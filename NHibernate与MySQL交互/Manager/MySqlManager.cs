using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Criterion;
using Zfstu.Model;
using Task = Zfstu.Model.Task;

namespace Zfstu.Manager
{
    public class MySqlManager<T> : IManager<T>
    {
        private static readonly object Locker = new object();

        private string tableName = null;
        public MySqlManager()
        {
            if (typeof(T) == typeof(User))
            {
                tableName = "Username";

            }else if (typeof(T) == typeof(UserInfo))
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
        public  bool Add(T t)
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
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
        }

        public bool AddOrUpdate(T t)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                using (var transaction = Session.BeginTransaction())
                {
                    try
                    {
                        Session.SaveOrUpdate(t);
                        transaction.Commit();
                        return true;
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
                var t = Session.CreateCriteria(typeof(T)).List<T>();
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

        public User GetUserByTid(string thirdId)
        {
            using (var Session = NHibernateHelper.OpenSession())
            {
                var user = Session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("ThirdId", thirdId))
                    .UniqueResult<User>();
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
    }
}
