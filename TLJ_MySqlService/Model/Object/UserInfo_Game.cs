using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class UserInfo_Game
{
    public string tag { get; set; }
    public int connId { get; set; }
    public string uid { get; set; }
    public int isClientReq { get; set; }
    public int code { get; set; }
    public string name { get; set; }
    public int vipLevel { get; set; }
    public int gold { get; set; }
    public int head { get; set; }
    public Gamedata gameData { get; set; }
    public List<UserBuff> BuffData { get; set; }

    public class Gamedata
    {
        public int allGameCount { get; set; }
        public int winCount { get; set; }
        public int runCount { get; set; }
        public int meiliZhi { get; set; }
    }

    public class UserBuff
    {
        public int prop_id { get; set; }
        public int buff_num { get; set; }
    }
}