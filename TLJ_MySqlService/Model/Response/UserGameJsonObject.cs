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
    public int xianxianJDPrimary { get; set; }
    public int xianxianJDMiddle { get; set; }
    public int xianxianJDHigh { get; set; }
    public int xianxianCDPrimary { get; set; }
    public int xianxianCDMiddle { get; set; }
    public int xianxianCDHigh { get; set; }

    public UserGameJsonObject(int allGameCount, int winCount, int runCount, int meiliZhi, int xianxianJdPrimary, int xianxianJdMiddle, int xianxianJdHigh, int xianxianCdPrimary, int xianxianCdMiddle, int xianxianCdHigh)
    {
        this.allGameCount = allGameCount;
        this.winCount = winCount;
        this.runCount = runCount;
        this.meiliZhi = meiliZhi;
        xianxianJDPrimary = xianxianJdPrimary;
        xianxianJDMiddle = xianxianJdMiddle;
        xianxianJDHigh = xianxianJdHigh;
        xianxianCDPrimary = xianxianCdPrimary;
        xianxianCDMiddle = xianxianCdMiddle;
        xianxianCDHigh = xianxianCdHigh;
    }
   
}