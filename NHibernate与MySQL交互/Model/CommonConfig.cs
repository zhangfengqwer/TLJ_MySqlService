namespace NhInterMySQL.Model
{
    public class CommonConfig
    {
        public virtual int id { set; get; }
        public virtual string Uid { set; get; }
        /// <summary>
        /// vip每天的充值限额
        /// </summary>
        public virtual int recharge_phonefee_amount { set; get; }
        /// <summary>
        /// 玩家一天花费的金币数
        /// </summary>
        public virtual int expense_gold_daily { set; get; }
        /// <summary>
        /// 微信公众号登陆发放神秘礼包，0：没有登陆过
        /// </summary>
        public virtual int wechat_login_gift { set; get; }
        /// <summary>
        /// 首充礼包
        /// </summary>
        public virtual int first_recharge_gift { set; get; }
        /// <summary>
        /// 3次补助金
        /// </summary>
        public virtual int free_gold_count { set; get; }
    }
}
