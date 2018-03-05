namespace NhInterMySQL.Model
{
    public class UserMonthSign
    {
        public virtual string Uid { get; set; }
        public virtual string SignYearMonth { get; set; }
        public virtual string SignDate { get; set; }

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
