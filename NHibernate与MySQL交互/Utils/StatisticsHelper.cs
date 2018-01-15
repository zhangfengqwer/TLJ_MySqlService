using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NhInterMySQL;
using NhInterMySQL.Model;


public class StatisticsHelper
{
    public static void StatisticsLogin(string uid)
    {
        //记录每个人的当天登陆次数
        CommonConfig commonConfig = ModelFactory.CreateConfig(uid);
        commonConfig.login_count_daily++;
        NHibernateHelper.commonConfigManager.Update(commonConfig);

        //第一次登陆的时候，统计加一
        if (commonConfig.login_count_daily == 1)
        {
            var statistics = ModelFactory.CreateStatistics();
            statistics.login_count++;
            NHibernateHelper.statisticsManager.Update(statistics);
        }
    }

    public static void StatisticsRegister(string uid)
    {
        //注册的时候算登录
        CommonConfig commonConfig = ModelFactory.CreateConfig(uid);
        commonConfig.login_count_daily++;
        NHibernateHelper.commonConfigManager.Update(commonConfig);

        //第一次登陆的时候，统计加一
        Statistics statistics = ModelFactory.CreateStatistics();
        statistics.register_count++;
        statistics.login_count++;
        NHibernateHelper.statisticsManager.Update(statistics);
    }

    public static void StatisticsRechargePerson(string uid)
    {
        //记录每个人的当天充值次数
        CommonConfig commonConfig = ModelFactory.CreateConfig(uid);
        commonConfig.recharge_count_daily++;
        NHibernateHelper.commonConfigManager.Update(commonConfig);

        //第一次充值的时候，统计充值人数加一
        if (commonConfig.recharge_count_daily == 1)
        {
            var statistics = ModelFactory.CreateStatistics();
            statistics.recharge_person_count++;
            NHibernateHelper.statisticsManager.Update(statistics);
        }
    }
}