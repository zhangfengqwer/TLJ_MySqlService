﻿namespace NhInterMySQL.Model
{
    public class User
    {
        public virtual int UserId { set; get; }
        public virtual string Username { set; get; }
        public virtual string Userpassword { set; get; }
        public virtual string Secondpassword { set; get; }
        public virtual string Uid { set; get; }
        public virtual string ThirdId { set; get; }
        public virtual int Platform { set; get; }
        public virtual int IsRobot { set; get; }
    }
}
