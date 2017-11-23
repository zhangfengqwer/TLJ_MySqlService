namespace NhInterMySQL.Model
{
    public class UserGame
    {
        public virtual string Uid { get; set; }
        public virtual int AllGameCount { get; set; }
        public virtual int DailyGameCount { get; set; }
        public virtual int WinCount { get; set; }
        public virtual int RunCount { get; set; }
        public virtual int MeiliZhi { get; set; }
        public virtual int XianxianJDPrimary { get; set; }
        public virtual int XianxianJDMiddle { get; set; }
        public virtual int XianxianJDHigh { get; set; }
        public virtual int XianxianCDPrimary { get; set; }
        public virtual int XianxianCDMiddle { get; set; }
        public virtual int XianxianCDHigh { get; set; }
    }
}
