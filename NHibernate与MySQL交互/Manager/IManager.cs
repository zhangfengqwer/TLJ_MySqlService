using System.Collections.Generic;
using NhInterMySQL.Model;

namespace NhInterMySQL.Manager
{
    interface IManager<T>
    {
        bool Add(T t);
        bool AddOrUpdate(T t);
        bool Delete(T t);
        bool Update(T user);
        T GetById(int id);
        T GetByName(string name);
        ICollection<T> GetAll();
        User VerifyLogin(string username, string password);
        UserEmail GetEmail(int emailId, string uid);
        UserProp GetUserProp(string uid, int propId);
    }
}
