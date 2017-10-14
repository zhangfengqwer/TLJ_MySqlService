using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class UserTask
    {
        public virtual string Uid { get; set; }
        public virtual int task_id { get; set; }
        public virtual int progress { get; set; }
        public virtual int isover { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
