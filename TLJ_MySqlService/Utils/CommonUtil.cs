using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TLJ_MySqlService;

public class CommonUtil
{
    public static int GetRandomSeed()
    {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static string GetFrontDirectory(string directory)
    {
        System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetParent(directory).FullName);
        directory = System.IO.Directory.GetCurrentDirectory();
        return directory;
    }

    /// <summary>
    /// 32位MD5加密
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetMD5(string password)
    {
        string cl = password;
        string pwd = "";
        MD5 md5 = MD5.Create(); //实例化一个md5对像
        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        for (int i = 0; i < s.Length; i++)
        {
            // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
            pwd = pwd + s[i].ToString("X2");
        }
        return pwd;
    }

    public static string CheckPsw(string password)
    {
        var md5 = GetMD5(MySqlService.AdminPassWord + password);
        return md5;
    }

//    public static string ChangePsw(string password)
//    {
//        var md5 = GetMD5(MySqlService.AdminPassWord + GetMD5(password));
//        return md5;
//    }
    // 格式2017/7/12 15:05:03
    static public string getCurTime()
    {
        return DateTime.Now.ToString();
    }

    // 格式2017/7
    static public string getCurYearMonth()
    {
        return DateTime.Now.Year + "/" + DateTime.Now.Month;
    }

    static public int getCurYear()
    {
        return DateTime.Now.Year;
    }

    static public int getCurMonth()
    {
        return DateTime.Now.Month;
    }

    static public int getCurDay()
    {
        return DateTime.Now.Day;
    }

    static public bool splitStrIsPerfect(string str, List<string> list, char c)
    {
        bool b = false;
        {
            if (str[str.Length - 1] == c)
            {
                b = true;
            }
        }

        splitStr(str, list, c);

        return b;
    }

    /*
     * 裁剪字符串：1.2.3.3.5
     * str：源字符串
     * list：裁剪后存放的地方
     * c：裁剪规则
     * 如：splitStr("1.2.3.4.5",list,'.');
     */
    static public void splitStr(string str, List<string> list, char c)
    {
        string temp = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != c)
            {
                temp += str[i];
            }
            else
            {
                list.Add(temp);
                temp = "";
            }

            if ((str[i] != c) && (i == (str.Length - 1)))
            {
                list.Add(temp);
                temp = "";
            }
        }
    }

    /*
     * subStringEndByChar("1/2/3/4/5/6",'/')
     * 返回6
     */
    static public string subStringEndByChar(string str, char c)
    {
        return str.Substring(str.LastIndexOf(c) + 1);
    }

    // size：物品数量
    // jiange：物品间隔
    // index：物品序号（从0开始）
    // centerPosX：居中位置坐标
    static public int getPosX(int size, int jiange, int index, int centerPosX)
    {
        int firstX;
        if (size % 2 == 0)
        {
            firstX = (centerPosX - jiange / 2) - (size / 2 - 1) * jiange;
        }
        else
        {
            firstX = centerPosX - (size / 2) * jiange;
        }

        return firstX + jiange * index;
    }
}