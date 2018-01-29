namespace NhInterMySQL.Model
{
    public class Log_Online_Player
    {
        public virtual int id { get; set; }
        public virtual string Uid { get; set; }
        public virtual string ip { get; set; }
        public virtual string channel_name { get; set; }
        public virtual string version_name { get; set; }
        public virtual string gameroomtype { get; set; }
        public virtual int room_id { get; set; }
        public virtual int is_robot { get; set; }
    }

    public class OnlinePlayerReq
    {
        public string tag { get; set; }
        public string uid { get; set; }
        public int room_id { get; set; }
        public string gameroomtype { get; set; }
        public bool isAI { get; set; }
        public int type { get; set; }
    }
}