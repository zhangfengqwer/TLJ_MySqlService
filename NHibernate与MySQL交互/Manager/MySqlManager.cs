using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Criterion;
using Zfstu.Model;

namespace Zfstu.Manager
{
    public class MySqlManager<T> : IManager<T>
    {
        private string tableName = null;
        public MySqlManager()
        {
            if (typeof(T) == typeof(User))
            {
                tableName = "Username";

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
                        Session.Save(t);
                        transaction.Commit();
                        return true;
                    }
                    catch(Exception e)
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
                        Session.Update(t);
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
    }
}
