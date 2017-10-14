using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class Task
    {
        public virtual int task_id { get; set; }
        public virtual string title { get; set; }
        public virtual string content { get; set; }
        public virtual string reward { get; set; }
        public virtual int target { get; set; }
    }
}
