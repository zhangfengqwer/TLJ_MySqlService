using System;

namespace NhInterMySQL.Model
{
    public class User
    {
        public virtual int UserId { set; get; }
        public virtual string Username { set; get; }
        public virtual string Userpassword { set; get; }
        public virtual string Secondpassword { set; get; }
        public virtual string Uid { set; get; }
        public virtual string ThirdId { set; get; }
        public virtual string ChannelName { set; get; }
        public virtual DateTime CreateTime { set; get; }
        public virtual int IsRobot { set; get; }
        public virtual string MachineId { set; get; }
    }
}
