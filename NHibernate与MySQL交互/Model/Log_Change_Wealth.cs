namespace NhInterMySQL.Model
{
    public class Log_Change_Wealth
    {
        public virtual int id { get; set; }
        public virtual string Uid { get; set; }
        public virtual string nick_name { get; set; }
        public virtual string type { get; set; }
        public virtual int before_num { get; set; }
        public virtual int change_num { get; set; }
        public virtual int after_num { get; set; }
        public virtual string reason { get; set; }
    }
}
