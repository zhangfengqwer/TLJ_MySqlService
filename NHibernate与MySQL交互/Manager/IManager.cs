using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zfstu.Model;

namespace Zfstu.Manager
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
