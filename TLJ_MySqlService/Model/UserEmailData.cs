using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.Model
{
    public class UserEmailData
    {
        public string tag;
        public int code;
        public int connId;
        public List<mailData> mailData;
    }

    public class mailData
    {
        public int email_id;
        public int state;
        public string title;
        public string content;
        public string reward;
        public string time; 
    }
}
