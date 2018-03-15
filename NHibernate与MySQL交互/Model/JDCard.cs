namespace NhInterMySQL.Model
{
    public class JDCard
    {
        public virtual int id { set; get; }
        public virtual string name { set; get; }
        public virtual string Uid { set; get; }
        public virtual string price { set; get; }
        public virtual string card_number { set; get; }
        public virtual string card_secret { set; get; }
        public virtual string valid_time { set; get; }
        public virtual string state { set; get; }
    }
}
