namespace Zfstu.Model
{
    public class UserProp
    {
        public virtual string Uid { set; get; }
        public virtual int PropId { set; get; }
        public virtual int PropNum { set; get; }
        //0:能使用
        public virtual int Type { set; get; }

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
