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
    }
}