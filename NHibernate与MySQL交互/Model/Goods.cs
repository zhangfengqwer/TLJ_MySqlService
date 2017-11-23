namespace NhInterMySQL.Model
{
    public class Goods
    {
        public virtual int goods_id { set; get; }
        public virtual int goods_type { set; get; }
        public virtual string props { set; get; }
        public virtual string goods_name { set; get; }
        public virtual int price { set; get; }
        public virtual int money_type { set; get; }
    }
}
