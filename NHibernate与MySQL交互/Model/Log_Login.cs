namespace NhInterMySQL.Model
{
    public class Log_Login
    {
        public virtual int id { get; set; }
        public virtual string Uid { get; set; }
        public virtual string ip { get; set; }
        public virtual string channel_name { get; set; }
        public virtual string nick_name { get; set; }
        public virtual string login_type { get; set; }
        public virtual string version_name { get; set; }
    }
}
