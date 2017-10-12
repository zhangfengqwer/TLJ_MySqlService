using System;
using System.Collections.Generic;

namespace Zfstu.Model
{
    public class User
    {
        public virtual int UserId { set; get; }
        public virtual string Username { set; get; }
        public virtual string Userpassword { set; get; }
        public virtual string Uid { set; get; }

        public virtual Sign USign { set; get; }
    }
}
