namespace NhInterMySQL.Model
{
    public class Log_bind_oldplayer
    {
        public virtual int id { get; set; }
        public virtual string Uid { get; set; }
        public virtual string old_uid { get; set; }
        public virtual string channel_name { get; set; }
        public virtual string machine_id { get; set; }
    }
}
