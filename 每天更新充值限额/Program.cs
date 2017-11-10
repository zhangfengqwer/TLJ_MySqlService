using System;
using System.Threading;
using System.Threading.Tasks;
using Zfstu;

namespace 每天更新充值限额
{
    class Program
    {
        static void Main(string[] args)
        {
            new Task(() =>
            {
                UpdateRechargeDayly();
            }).Start();
        }

       
    }
}
