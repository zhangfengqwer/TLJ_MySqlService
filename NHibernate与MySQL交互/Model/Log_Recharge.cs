namespace NhInterMySQL.Model
{
    public class Log_Recharge
    {
        public virtual string order_id { get; set; }
        public virtual string uid { get; set; }
        public virtual string nick_name { get; set; }
        public virtual string goods_id { get; set; }
        public virtual string goods_num { get; set; }
        public virtual string price { get; set; }
    }
}
