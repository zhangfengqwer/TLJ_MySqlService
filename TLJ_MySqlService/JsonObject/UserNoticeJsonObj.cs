using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.JsonObject
{
    public class UserNoticeJsonObj
    {
        public int notice_id;
        public string title;
        public string content;
        public int type;
        public int state;
        public DateTime start_time;
        public DateTime end_time;
    }
}
