using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhInterSqlServer.Model
{
    public class UserBindPhone
    {
        public virtual int UserId { get; set; }
        public virtual string Mobile { get; set; }
        public virtual DateTime ModifyTime { get; set; }
    }
}
