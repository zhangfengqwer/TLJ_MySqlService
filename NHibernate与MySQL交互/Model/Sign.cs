using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class Sign
    {
        public virtual string Uid { get; set; }
        public virtual int SignWeekDays { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
