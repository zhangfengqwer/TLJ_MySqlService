using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NhInterMySQL.Model;

namespace NhInterMySQL
{
    public class StatictisLogUtil
    {
        public static void Login(string uid, string nickName, string ip, string channelName, string versionName,
            string loginType)
        {
            Log_Login logLogin = new Log_Login()
            {
                Uid = uid,
                ip = ip,
                channel_name = channelName,
                version_name = versionName,
                login_type = loginType,
                nick_name = nickName
            };
            NHibernateHelper.LogLoginManager.Add(logLogin);
        }

        public static void Recharge(string uid, string nickName, string orderId, string goodsId, string goodsNum,
            string price)
        {
            Log_Recharge recharge = new Log_Recharge()
            {
                uid = uid,
                nick_name = nickName,
                order_id = orderId,
                goods_id = goodsId,
                goods_num = goodsNum,
                price = price
            };
            NHibernateHelper.LogRechargeManager.Add(recharge);
        }

        public static void Online(string uid, string ip, string channelName, string versionName, string roomtype,
            int roomId, int isRobot)
        {
            Log_Online_Player onlinePlayer = new Log_Online_Player()
            {
                Uid = uid,
                ip = ip,
                channel_name = channelName,
                version_name = versionName,
                gameroomtype = roomtype,
                room_id = roomId,
                is_robot = isRobot,
            };
            NHibernateHelper.LogOnlinePlayerManager.Add(onlinePlayer);
        }

        public static void ChangeWealth(string uid, string nickname, string type, string reason, int before, int change,
            int after)
        {
            Log_Change_Wealth logChangeWealth = new Log_Change_Wealth()
            {
                Uid = uid,
                nick_name = nickname,
                type = type,
                reason = reason,
                before_num = before,
                change_num = change,
                after_num = after
            };

            NHibernateHelper.LogChangeWealthManager.Add(logChangeWealth);

        }

        public static void Game(int roomid, string gamename, string play1Uid, string play2Uid, string play3Uid,
            string play4Uid,int curpvpround ,string win1,string win2,string zhuangUid)
        {
            Log_Game logGame = new Log_Game()
            {
                roomid = roomid,
                player1_uid = play1Uid,
                player2_uid = play2Uid,
                player3_uid = play3Uid,
                player4_uid = play4Uid,
                cur_pvp_round = curpvpround,
                gameroomname = gamename,
                winner1_uid = win1,
                winner2_uid = win2,
                zhuangjia_uid = zhuangUid
            };

            NHibernateHelper.LogGameManager.Add(logGame);
        }
    }
}