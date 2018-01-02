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
        var commonConfig = new CommonConfig();
        commonConfig.Uid = uid;
        commonConfig.recharge_phonefee_amount = 0;
        commonConfig.wechat_login_gift = 0;
        commonConfig.first_recharge_gift = 0;
        commonConfig.expense_gold_daily = 0;
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

    public static User CreateUser(string uid)
    {
        var user = new User()
        {
            Uid = uid,
            IsRobot = 0,
            Platform = 0,
            Secondpassword ="",
            ThirdId ="",
            Username = uid,
            Userpassword = uid
        };

        NHibernateHelper.userManager.Add(user);

        return user;
    }

}