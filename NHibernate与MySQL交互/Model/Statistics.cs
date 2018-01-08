using System;

namespace NhInterMySQL.Model
{
    public class Statistics
    {
        public virtual int id { set; get; }
        public virtual string time { set; get; }
        public virtual int login_count { set; get; }
        public virtual int register_count { set; get; }
        public virtual float recharge_total { set; get; }
        public virtual int recharge_person_count { set; get; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
