using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TimeHelper
{
    public static string GetCurrentHour()
    {
        return DateTime.Now.ToString("yyyy-MM-dd hh");
    }
}