namespace NhInterMySQL.Model
{
    public class MyLog
    {
        public virtual int log_id { get; set; }
        public virtual string uid { get; set; }
        public virtual string optype { get; set; }
        public virtual string message { get; set; }
    }
}
