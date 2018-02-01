using Newtonsoft.Json;
namespace NhInterMySQL.Model
{
    public class Log_Game
    {
        public virtual int id { get; set; }
        public virtual string tag { get; set; }
        public virtual int roomid { get; set; }
        public virtual string gameroomname { get; set; }
        public virtual string player1_uid { get; set; }
        public virtual string player2_uid { get; set; }
        public virtual string player3_uid { get; set; }
        public virtual string player4_uid { get; set; }
        public virtual string zhuangjia_uid { get; set; }
        public virtual string winner1_uid { get; set; }
        public virtual string winner2_uid { get; set; }
        public virtual string pvp_reward { get; set; }
        public virtual int cur_pvp_round { get; set; }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(tag)}: {tag}, {nameof(roomid)}: {roomid}, {nameof(gameroomname)}: {gameroomname}, {nameof(player1_uid)}: {player1_uid}, {nameof(player2_uid)}: {player2_uid}, {nameof(player3_uid)}: {player3_uid}, {nameof(player4_uid)}: {player4_uid}, {nameof(zhuangjia_uid)}: {zhuangjia_uid}, {nameof(winner1_uid)}: {winner1_uid}, {nameof(winner2_uid)}: {winner2_uid}, {nameof(pvp_reward)}: {pvp_reward}, {nameof(cur_pvp_round)}: {cur_pvp_round}";
        }
    }
}
