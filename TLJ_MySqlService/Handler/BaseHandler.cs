using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.Handler
{
    public abstract class BaseHandler
    {
        public abstract string OnResponse(string data);
    }
}
