namespace NhInterMySQL.Model
{
    public class Log_Login_old
    {
        public virtual string uid_old { get; set; }
        public virtual int game_id { get; set; }
        public virtual string channel_name { get; set; }
        public virtual string machine_id { get; set; }
        public virtual string version_name { get; set; }

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
