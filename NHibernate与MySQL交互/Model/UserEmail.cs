using System;
using System.Collections.Generic;

namespace Zfstu.Model
{
    public class UserEmail
    {
        public virtual int EmailId { set; get; }
        public virtual string Uid { set; get; }
        public virtual string Title { set; get; }
        public virtual string Content { set; get; }
        public virtual string Reward { set; get; }
        public virtual int State { set; get; }
        public virtual DateTime CreateTime { set; get; }
    }
}
