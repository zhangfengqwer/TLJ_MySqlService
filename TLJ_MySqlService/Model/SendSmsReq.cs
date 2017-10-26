using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.Model
{
    class SendSmsReq
    {
        public string tag;
        public int connId;
        public string uid;
        public string phoneNum;
    }
}
