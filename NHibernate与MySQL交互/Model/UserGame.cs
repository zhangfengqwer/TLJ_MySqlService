﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zfstu.Model
{
    public class UserGame
    {
        public virtual string Uid { get; set; }
        public virtual int AllGameCount { get; set; }
        public virtual int WinCount { get; set; }
        public virtual int RunCount { get; set; }
        public virtual int MeiliZhi { get; set; }
    }
}
