using System;

namespace NhInterMySQL.Model
{
    public class UserPhoneExchange
    {
        public virtual string Uid { set; get; }
        public virtual int Id { set; get; }
        public virtual string Phone { set; get; }
        public virtual int Money { set; get; }
        public virtual DateTime CreateTime { set; get; }
    }
}
