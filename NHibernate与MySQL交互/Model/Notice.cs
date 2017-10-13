using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class Notice
    {
        public virtual int NoticeId { set; get; }
        public virtual string Title { set; get; }
        public virtual string Content { set; get; }
        public virtual int Type { set; get; }
        public virtual DateTime StartTime { set; get; }
        public virtual DateTime EndTime { set; get; }
    }
}
