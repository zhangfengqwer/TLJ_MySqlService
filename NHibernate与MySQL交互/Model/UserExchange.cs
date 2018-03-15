using System;
using Newtonsoft.Json;

namespace NhInterMySQL.Model
{
    public class UserExchange
    {
        [JsonIgnore]
        public virtual string Uid { get; set; }
        public virtual int goods_id { get; set; }
        public virtual int num { get; set; }
        [JsonIgnore]
        public virtual DateTime create_time { get; set; }
        [JsonIgnore]
        public virtual int medal_cost { get; set; }
        public virtual string time { get; set; }
        public virtual string day { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
