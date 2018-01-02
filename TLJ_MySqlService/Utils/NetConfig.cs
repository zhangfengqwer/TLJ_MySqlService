using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class NetConfig
{
    public static string s_loginService_ip;
    public static int s_loginService_port;

    public static string s_mySqlService_ip;
    public static int s_mySqlService_port;

    public static void init()
    {
        try
        {
            // 读取文件
            {
                StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "NetConfig.json");
                string str = sr.ReadToEnd().ToString();
                sr.Close();

                JObject jo = JObject.Parse(str);

                // 登录服务器
                {
                    JObject j = (JObject)jo.GetValue("LoginService");
                    s_loginService_ip = j.GetValue("ip").ToString();
                    s_loginService_port = Convert.ToInt32(j.GetValue("port"));
                }

                // 数据库服务器
                {
                    JObject j = (JObject)jo.GetValue("MySqlService");
                    s_mySqlService_ip = j.GetValue("ip").ToString();
                    s_mySqlService_port = Convert.ToInt32(j.GetValue("port"));
                }

            }

        }
        catch(Exception ex)
        {
            LogUtil.getInstance().writeLogToLocalNow("读取网络配置文件出错：" + ex.Message);
        }
    }
}
