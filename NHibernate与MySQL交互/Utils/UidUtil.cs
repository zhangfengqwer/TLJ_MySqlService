using System;


public  class UidUtil
    {
    // 生成UID
        public static string createUID()
        {
            string timeStamp = "";
            timeStamp = GetTimeStamp();
            timeStamp = ("6" + timeStamp.Substring(1));

            Random ran = new Random();
            int i = ran.Next(100, 99999);
            long temp = Convert.ToInt64(timeStamp) + i;

            return temp.ToString();
        }

        // 获取时间戳：秒级
        private static  string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

}
