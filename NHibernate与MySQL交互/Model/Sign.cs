using System;

namespace NhInterMySQL.Model
{
    public class Sign
    {
        public virtual string Uid { get; set; }
        public virtual int SignWeekDays { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
