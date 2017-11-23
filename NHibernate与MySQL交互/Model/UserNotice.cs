namespace NhInterMySQL.Model
{
    public class UserNotice
    {
        public virtual string Uid { set; get; }
        public virtual int NoticeId { set; get; }
        public virtual int State { set; get; }
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
