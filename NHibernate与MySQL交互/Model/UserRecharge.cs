namespace NhInterMySQL.Model
{
    public class UserRecharge
    {
        public virtual string Uid { get; set; }
        public virtual int goods_id { get; set; }
        public virtual int recharge_count { get; set; }

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
