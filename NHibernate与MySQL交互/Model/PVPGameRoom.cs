using System;
using System.Collections.Generic;

namespace Zfstu.Model
{
    public class PVPGameRoom
    {
        public virtual int id { set; get; }
        public virtual string gameroomtype { set; get; }
        public virtual string gameroomname { set; get; }
        public virtual int kaisairenshu { set; get; }
        public virtual string reward { set; get; }
        public virtual string baomingfei { set; get; }
    }
}