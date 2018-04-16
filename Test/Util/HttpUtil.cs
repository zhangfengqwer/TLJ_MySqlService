using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.Utils
{
    public class HttpUtil
    {
        //线上
        //private static string phoneFeeKey = "fw123";
        //private static string clientip = "139.196.193.185";
        //测试

        public static string sendKey = "sy";
        public static string phoneFeeKey = "sy";
        public static string clientip = "58.210.102.138";

        private static string gameid = "210";
        private static string flatFrom = "70";

        //body是要传递的参数,格式"roleId=1&uid=2"
        //post的cotentType填写:
        //"application/x-www-form-urlencoded"
        //soap填写:"text/xml; charset=utf-8"
        public static async Task<string> PostHttp(string url, string body)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 10000;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        public static async Task<string> GetHttp(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);

            httpWebRequest.ContentType = "text/xml; charset=utf-8";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 10000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }
    }
}