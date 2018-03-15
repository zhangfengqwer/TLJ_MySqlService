using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NhInterMySQL;
using NhInterMySQL.Model;

public class ModelFactory
{
    public static CommonConfig CreateConfig(string uid)
    {
        var commonConfig = NHibernateHelper.commonConfigManager.GetByUid(uid);
        if (commonConfig != null)
        {
            return commonConfig;
        }

        commonConfig = new CommonConfig();
        commonConfig.Uid = uid;
        commonConfig.recharge_phonefee_amount = 0;
        commonConfig.wechat_login_gift = 0;
        commonConfig.first_recharge_gift = 0;
        commonConfig.expense_gold_daily = 0;
        commonConfig.login_count_daily = 0;
        commonConfig.recharge_count_daily = 0;
        commonConfig.free_gold_count = 3;

        NHibernateHelper.commonConfigManager.Add(commonConfig);

        return commonConfig;
    }

    public static UserRecharge CreateUserRecharge(string uid,int goodId)
    {
        var userRecharge = new UserRecharge()
        {
            Uid = uid,
            goods_id = goodId,
            recharge_count = 0,
        };

        NHibernateHelper.userRechargeManager.Add(userRecharge);

        return userRecharge;
    }

    public static User CreateUser(string uid,string channelName)
    {
        var user = new User()
        {
            Uid = uid,
            IsRobot = 0,
            ChannelName = channelName,
            Secondpassword ="",
            ThirdId ="",
            Username = uid,
            Userpassword = uid,
            CreateTime = DateTime.Now
        };

        NHibernateHelper.userManager.Add(user);

        return user;
    }

    public static Statistics CreateStatistics()
    {
        Statistics statisByHour = NHibernateHelper.statisticsManager.GetStatisByHour(TimeHelper.GetCurrentHour());

        if (statisByHour != null)
        {
            return statisByHour;
        }
        Statistics statistics = new Statistics()
        {
            login_count = 0,
            recharge_total = 0,
            recharge_person_count = 0,
            register_count = 0,
            time = DateTime.Now.ToString("yyyy-MM-dd HH"),
        };
        NHibernateHelper.statisticsManager.Add(statistics);

        return statistics;
    }
}