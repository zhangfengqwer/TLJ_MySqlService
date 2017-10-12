using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UserGameJsonObject
{
    public int allGameCount { get; set; }
    public int winCount { get; set; }
    public int runCount { get; set; }
    public int meiliZhi { get; set; }

    public UserGameJsonObject(int allGameCount, int winCount, int runCount, int meiliZhi)
    {
        this.allGameCount = allGameCount;
        this.winCount = winCount;
        this.runCount = runCount;
        this.meiliZhi = meiliZhi;
    }
}