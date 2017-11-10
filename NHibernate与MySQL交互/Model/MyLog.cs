using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class MyLog
    {
        public virtual int log_id { get; set; }
        public virtual string uid { get; set; }
        public virtual string optype { get; set; }
        public virtual string message { get; set; }
    }
}
