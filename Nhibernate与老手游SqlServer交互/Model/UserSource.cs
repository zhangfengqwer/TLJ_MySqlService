using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhInterSqlServer.Model
{
    public class UserSource
    {
        public virtual int UserId { get; set; }
        public virtual string SourceCode { get; set; }
        public virtual int GameID { get; set; }
    }
}
