﻿namespace NhInterMySQL.Model
{
    public class UserInfo
    {
        public virtual string Uid { set; get; }
        public virtual string NickName { set; get; }
        public virtual int Head { set; get; }
        public virtual int Gold { set; get; }
        public virtual int Medel { set; get; }
        public virtual int YuanBao { set; get; }
        public virtual int RechargeVip { set; get; }
        public virtual int freeCount { set; get; }
        public virtual int huizhangCount { set; get; }
        public virtual int luckyValue { set; get; }
        public virtual string Phone { set; get; }
        public virtual string ExtendCode { set; get; }
    }
}
