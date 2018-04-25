using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using NhInterMySQL.Manager;
using NhInterSqlServer;
using NhInterSqlServer.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TLJ_MySqlService.Handler;
using TLJ_MySqlService.Utils;
using Util;
using Task = System.Threading.Tasks.Task;


namespace Test
{
    class Program
    {
        private static HashSet<string> hashSet = new HashSet<string>();
        private static List<String> xingList = new List<string>();
        private static List<String> maleList = new List<string>();
        private static List<String> femaleList = new List<string>();

        private static string privateJavaKey =
            @"MIIBVAIBADANBgkqhkiG9w0BAQEFAASCAT4wggE6AgEAAkEAgJ7GQfseQ1XPXqF4pxNOkYIjOEVV5AAJ7RnGZtTOt+elwjIO3rtxascOJZtoLjkKi2VBISmDVUNxifsipAdg3wIDAQABAkBcH91HUzOI7UR7tlIx8U08QacyXc84YKK7ddO6wcBSzg+7A9COQotVft6vRU5hDVNrn6c5lRQYrCTGiUbfA8zhAiEAvBPKfJ/5eUFC5FyYH7o+/CAV0wcQGWToR58ssFqPQC8CIQCvEgdqLqQ/yk4kz4p4WbxDiyMsrFT4MBunSwOZ2gaOUQIgEgob6+Q0O4sk7V5sQO7OR8SUE0+kHatuFCCSWr/06YUCIQCJ3p3ePgr1fYFateKrcqezXXB+7twfc+tjLM0SLUP6cQIgcj0Nu0MeNEyZagFtWpC/OwO4v7ctrXDsovgtRsCdlns=";

        private static string publicJavaKey =
            @"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAICexkH7HkNVz16heKcTTpGCIzhFVeQACe0ZxmbUzrfnpcIyDt67cWrHDiWbaC45CotlQSEpg1VDcYn7IqQHYN8CAwEAAQ==";

        private static string content =
            "amount=0.01&applicationID=100174463&country=CN&currency=CNY&merchantId=900086000000034648&productDesc=shang pin miao shu&productName=ceshi shangpin&requestId=20180103032610997167248&sdkChannel=1&urlver=2";

        private static string content1 =
            "amount=1.00&applicationID=100174463&country=CN&currency=CNY&merchantId=900086000000034648&productDesc=10元宝&productName=10元宝&requestId=10310&sdkChannel=1&urlver=2";

        private static string result =
            @"W6GXLobPVb84rVK/rR0HcgqGHKNztsSql5tmoa8eBypWHxSlFoamUhAK+2/9ehXpNrWPDNfLlW0mP42mosR+Gg==";


        //        public static Dictionary<TljServiceType, IntPtr> serviceDic = new Dictionary<TljServiceType, IntPtr>();
        private static byte[] CombineBytes(byte[] data1, byte[] data2)
        {
            byte[] data = new byte[data1.Length + data2.Length];
            Buffer.BlockCopy(data1, 0, data, 0, data1.Length); //这种方法仅适用于字节数组
            Buffer.BlockCopy(data2, 0, data, data1.Length, data2.Length);
            return data;
        }

        private static HttpListener listener;
        private static TcpListener tcpListenerV4;
        private static TcpListener tcpListenerV6;

        public class ActivityData
        {
            public int ActivityId;
            public string Title;
            public string ImageUrl;
        }

        static void Main(string[] args)
        {
            //            List<UserBindPhone> userBindPhones = NHiMsServerteHelper.GetAll<UserBindPhone>();
            //            List<UserBindPhone> bindPhones270 = new List<UserBindPhone>();
            //            List<UserBindPhone> bindPhones218 = new List<UserBindPhone>();
            //            foreach (var phone in userBindPhones)
            //            {
            //                UserSource userSource = NHiMsServerteHelper.GetById<UserSource>(phone.UserId);
            //                if (userSource?.GameID == 270)
            //                {
            //                    bindPhones270.Add(phone);
            //                }
            //
            //                if (userSource?.GameID == 218)
            //                {
            //                    bindPhones218.Add(phone);
            //                }
            //            }
            //            Console.WriteLine(userBindPhones.Count);
            //            Console.WriteLine(bindPhones270.Count);
            //            Console.WriteLine(bindPhones218.Count);
            //
            //            using (FileStream fs = new FileStream("/../../OldGameUserPhone_218_2107_now.txt", FileMode.Create))
            //            {
            //                string serializeObject = JsonConvert.SerializeObject(bindPhones218);
            //                byte[] bytes = Encoding.ASCII.GetBytes(serializeObject);
            //                fs.Write(bytes, 0, bytes.Length);
            //            }
            //
            //            using (FileStream fs = new FileStream("/../../OldGameUserPhone_270_2107_now.txt", FileMode.Create))
            //            {
            //                string serializeObject = JsonConvert.SerializeObject(bindPhones270);
            //                byte[] bytes = Encoding.ASCII.GetBytes(serializeObject);
            //                fs.Write(bytes, 0, bytes.Length);
            //            }

            //            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

            List<UserMonthSign> userMonthSigns = GetSign30RecordHandler.GetSign30RecordSql("6515448709");
            Console.WriteLine(userMonthSigns.Count);

            Console.ReadLine();
        }

        private static void BubblingSort()
        {
            int[] intArray = {10, 0, 20, 50, 12, 32, 89, 10, 21};
            for (int i = 0; i < intArray.Length - 1; i++)
            {
                for (int j = i + 1; j < intArray.Length; j++)
                {
                    if (intArray[i] > intArray[j])
                    {
                        int temp = intArray[i];

                        intArray[i] = intArray[j];
                        intArray[j] = temp;
                    }
                }
            }

            for (int i = 0; i < intArray.Length; i++)
            {
                Console.WriteLine(intArray[i]);
            }
        }

        private static void NewMethod()
        {
            int i = 2;
            int j = i;
            j = 3;

            string s = "123";
            string s1 = s;
            s1 += "1";
            Console.WriteLine(i);
            Console.WriteLine(j);

            Console.WriteLine(s);
            Console.WriteLine(s1);
        }

        private static async void StartTest()
        {
//            HttpUtil.PostHttp("http://139.199.21.143/mLogin/WechatLogin", "123");

            AccountInfo accountInfo = new AccountInfo() {name = "13", pwd = "1231"};
            string s = await HttpUtil.PostHttp("http://127.0.0.1/mLogin/WechatLogin",
                "sdfsfsdf");

            Console.WriteLine(s);
        }

        public class AccountInfo
        {
            public string name;
            public string pwd;
        }

        private static void AddHttps()
        {
            X509Certificate2 cert =
                new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "/yybcer.pfx", "uaZRox1g");
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            if (!store.Certificates.Contains(cert))
            {
                store.Add(cert);
            }

            store.Close();
        }

        private static void initData()
        {
            string[] paths = Directory.GetFiles(configPath);

            foreach (var path in paths)
            {
                if (Path.GetExtension(path) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(path).StartsWith("~"))
                {
                    continue;
                }

                XSSFWorkbook xssfWorkbook;
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    xssfWorkbook = new XSSFWorkbook(file);
                    ISheet sheet = xssfWorkbook.GetSheetAt(0);
                    int num = sheet.GetRow(0).LastCellNum;
                    for (int i = 0; i < num; i++)
                    {
                        string s = sheet.GetRow(0).GetCell(i).ToString();
                        Console.WriteLine(s);
                    }
                }
            }
        }

        private static string configPath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Config";
        public static List<TurnTable> TurnTables;
        public static List<TurnTable> IosTurnTables;
        public static List<TurnTable> FreeTurnTables;
        public static List<TurnTable> MedalTurnTables;

        private static int GetProbabilityReward(List<TurnTable> turnTables)
        {
            //            var probability1 = 32.6 * 100;
            //            var probability2 = 9.9 * 100;
            //            var probability3 = 6.89 * 100;
            //            var probability4 = 1 * 100;
            //            var probability5 = 25.49 * 100;
            //            var probability6 = 3 * 100;
            //            var probability7 = 9.6 * 100;
            //            var probability8 = 7.15 * 100;
            //            var probability9 = 5.32 * 100;
            //            var probability10 = 0.05 * 100;
            var doubles = new List<double>();
            foreach (var turnTable in turnTables)
            {
                doubles.Add(turnTable.probability);
            }

            var list = new List<double>();
            for (int i = 0; i < doubles.Count; i++)
            {
                double temp = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    temp += doubles[j];
                }

                list.Add(temp);
            }

            int next = new Random(CommonUtil.GetRandomSeed()).Next(1, 10001);

            Console.WriteLine(next);

            int num = 0;
            if (next <= list[0]) num = 1;
            else if (next <= list[1])
                num = 2;
            else if (next <= list[2])
                num = 3;
            else if (next <= list[3])
                num = 4;
            else if (next <= list[4])
                num = 5;
            else if (next <= list[5])
                num = 6;
            else if (next <= list[6])
                num = 7;
            //            else if (next <= list[7]) num = 8;
            //            else if (next <= list[8]) num = 9;
            //            else if (next <= list[9]) num = 10;

            return num;
        }

        public static string FileMD5(string filePath)
        {
            byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
            }

            return retVal.ToHex("x2");
        }

        /// <summary>
        /// 对字典进行排序 appid=xxx&secret=xxx
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="args">不参与排序的key值</param>
        /// <returns></returns>
        public static string SortDictionary(Dictionary<string, string> dictionary, params string[] args)
        {
            List<string> keys = new List<string>();
            keys.AddRange(dictionary.Keys);
            keys.Sort();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (args.Contains(key)) continue;
                dictionary.TryGetValue(key, out var value);
                sb.Append(key);
                sb.Append("=");
                sb.Append(value);
                if (i != keys.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }


        private static IPAddress GetIpFromHost(string host)
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostEntry.AddressList[0];

            return ipAddress;
        }

        private static void StartV4AndV6()
        {
            int port = 9900;
            tcpListenerV4 = new TcpListener(IPAddress.Any, port);
            tcpListenerV6 = new TcpListener(IPAddress.IPv6Any, port);
            if (Socket.OSSupportsIPv6)
            {
                Console.WriteLine("支持v6");
            }

            if (Socket.OSSupportsIPv4)
            {
                Console.WriteLine("支持v4");
            }

            tcpListenerV4.Start();
            tcpListenerV6.Start();
            StartReceiveV4();
            StartReceiveV6();
        }

        private static async void StartReceiveV4()
        {
            while (true)
            {
                TcpClient tcpClientV4 = await tcpListenerV4.AcceptTcpClientAsync();
                Console.WriteLine("v4连接");
                SocketReceive(tcpClientV4);
            }
        }

        private static async void StartReceiveV6()
        {
            while (true)
            {
                TcpClient tcpClientV6 = await tcpListenerV6.AcceptTcpClientAsync();
                Console.WriteLine("v6连接");
                SocketReceive(tcpClientV6);
            }
        }

        private static void SocketReceive(TcpClient tcpClient)
        {
            string ip = tcpClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("ip:" + ip);
        }

        private static void StartHttp()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1/");
            listener.Start();

            Accept();

            Console.WriteLine($"start");
            Console.ReadKey();
        }

        public static async void Accept()
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                string absoluteUri = context.Request.Url.AbsoluteUri;
                string LocalPath = context.Request.Url.LocalPath;
                string AbsolutePath = context.Request.Url.AbsolutePath;
                Console.WriteLine($"absoluteUri:{absoluteUri}\nLocalPath:{LocalPath}\nAbsolutePath:{AbsolutePath}");
                string s = context.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine($"ip:{s}");
                //                string rawUrl = context.Request.RawUrl;
                //                string contentType = context.Request.ContentType;
                //                Encoding contentEncoding = context.Request.ContentEncoding;
                //                string httpMethod = context.Request.HttpMethod;
                //                NameValueCollection headers = context.Request.Headers;
                //
                //                Console.WriteLine(
                //                    $"rawUrl:{rawUrl}\ncontentType:{contentType}\ncontentEncoding:{contentEncoding}\nhttpMethod;{httpMethod}");

                //                foreach (var key in headers.AllKeys)
                //                {
                //                    string[] strings = headers.GetValues(key);
                //                    Console.WriteLine($"key:{key},Values:{strings?.ToString()}");
                //                }

                //                string type = context.Request.QueryString["type"];
                //                string contents = context.Request.QueryString["content"];
                //                Console.WriteLine($"type:{type}\ncontent:{contents}");
                Stream inputStream = context.Request.InputStream;
                string contentType = context.Request.ContentType;
                long length64 = context.Request.ContentLength64;
                Console.WriteLine($"contentType:{contentType}");
                byte[] bytes1 = new byte[length64];
                for (int offset = 0; offset < length64;)
                {
                    int i = await inputStream.ReadAsync(bytes1, 0, bytes1.Length - offset);

                    offset += i;
                }

                string data = Encoding.UTF8.GetString(bytes1);

                Order order = JsonConvert.DeserializeObject<Order>(data);

                Console.WriteLine($"data:{data}");

                Stream outputStream = context.Response.OutputStream;
                string response = $"<HTML><BODY>type:,content</BODY></HTML>";

                byte[] bytes = Encoding.UTF8.GetBytes(response);
                context.Response.ContentLength64 = bytes.Length;
                context.Response.ContentEncoding = Encoding.UTF8;
                await outputStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public class Order
        {
            public string total_amount { get; set; }
            public string gameId { get; set; }
            public string codePayId { get; set; }
            public string price { get; set; }
            public string userId { get; set; }
            public string version { get; set; }
            public string ProductNum { get; set; }
            public string ProductName { get; set; }
            public string appId { get; set; }
            public string expand { get; set; }
            public string ProductDesc { get; set; }
            public string ProductId { get; set; }
            public string PhoneModel { get; set; }
        }

        public static void IpConfig()
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(@"fksq.javgame.com");
            IPAddress address = ipHostEntry.AddressList[0];
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                Console.WriteLine("ipv4");
            }
            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Console.WriteLine("ipv6");
            }

            string ip = address.ToString();
            Console.WriteLine(ip);
        }

        public static async void test2()
        {
            await Test1();
        }

        public static async Task Test1()
        {
            for (int i = 0; i < 100; i++)
            {
                new System.Threading.Tasks.Task(() =>
                {
                    StatictisLogUtil.Login("123", "123", "ip", "channel", "123", "login");
                }).Start();
            }
        }

        //
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

        public static string ChangePsw(string password)
        {
            var md5 = GetMD5("jinyou123" + GetMD5(password));
            return md5;
        }

        public static string CheckPsw(string password)
        {
            var md5 = GetMD5("jinyou123" + password);
            return md5;
        }

        private static string GetName()
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "RandomName.json");
            string str = sr.ReadToEnd().ToString();
            sr.Close();

            JObject jo = JObject.Parse(str);

            List<string> xingList = JsonConvert.DeserializeObject<List<String>>(jo["xing"].ToString());
            List<string> maleList = JsonConvert.DeserializeObject<List<String>>(jo["maleMing"].ToString());
            List<string> femaleList = JsonConvert.DeserializeObject<List<String>>(jo["femaleMing"].ToString());
            int next = new Random(GetRandomSeed()).Next(0, xingList.Count);
            string xing = xingList[next];
            string ming = "";
            int next1 = new Random(GetRandomSeed()).Next(0, 2);
            switch (next1)
            {
                case 0:
                    int next2 = new Random(GetRandomSeed()).Next(0, maleList.Count);
                    ming = maleList[next2];
                    break;
                case 1:
                    int next3 = new Random(GetRandomSeed()).Next(0, femaleList.Count);
                    ming = femaleList[next3];
                    break;
            }

            return xing + ming;
        }


        private static StringBuilder BuildAISql()
        {
            var aiList = NHibernateHelper.userManager.GetAIList();
            StringBuilder sb = new StringBuilder();
            foreach (var ai in aiList)
            {
                string temp =
                    $"({ai.UserId},'{ai.Uid}','{ai.Username}','{ai.Userpassword}','{ai.Secondpassword}','{ai.ThirdId}','{ai.ChannelName}','{ai.IsRobot}'),";
                sb.Append(temp);
            }

            return sb;
        }


        private static string GetRandomName()
        {
            DataTable dataTable = GetDataFromExcelWithAppointSheetName("d:\\手游随机名字库.xls");
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow dataTableRow = dataTable.Rows[i];

                if (!string.IsNullOrWhiteSpace(dataTableRow[0].ToString()))
                {
                    xingList.Add(dataTableRow[0].ToString());
                }

                if (!string.IsNullOrWhiteSpace(dataTableRow[2].ToString()))
                {
                    maleList.Add(dataTableRow[2].ToString());
                }

                if (!string.IsNullOrWhiteSpace(dataTableRow[4].ToString()))
                {
                    femaleList.Add(dataTableRow[4].ToString());
                }
            }


            int next = new Random(GetRandomSeed()).Next(0, xingList.Count);
            string xing = xingList[next];
            string ming = "";
            int next1 = new Random(GetRandomSeed()).Next(0, 2);
            switch (next1)
            {
                case 0:
                    int next2 = new Random(GetRandomSeed()).Next(0, maleList.Count);
                    ming = maleList[next2];
                    break;
                case 1:
                    int next3 = new Random(GetRandomSeed()).Next(0, femaleList.Count);
                    ming = femaleList[next3];
                    break;
            }

            return xing + ming;
        }

        /// <summary>
        /// 根据excel的文件的路径提取其中表的数据
        /// </summary>
        /// <param name="Path">Excel文件的路径</param>
        private static DataTable GetDataFromExcelWithAppointSheetName(string Path)
        {
            //连接串
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" +
                             "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();

            //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等  
            DataTable dtSheetName =
                conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "Table"});

            //包含excel中表名的字符串数组
            string[] strTableNames = new string[dtSheetName.Rows.Count];
            for (int k = 0; k < dtSheetName.Rows.Count; k++)
            {
                strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
            }

            OleDbDataAdapter myCommand = null;
            DataTable dt = new DataTable();

            //从指定的表明查询数据,可先把所有表明列出来供用户选择
            string strExcel = "select * from [" + strTableNames[0] + "]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            dt = new DataTable();
            myCommand.Fill(dt);
            return dt;
            //dataGridView1.DataSource = dt; //绑定到界面
        }


        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static int GetProbabilityReward()
        {
            var probability1 = 32.6 * 100;
            var probability2 = 10.15 * 100;
            var probability3 = 5.89 * 100;
            var probability4 = 1 * 100;
            var probability5 = 25.49 * 100;
            var probability6 = 6 * 100;
            var probability7 = 9.6 * 100;
            var probability8 = 6.9 * 100;
            var probability9 = 2.32 * 100;
            var probability10 = 0.05 * 100;
            var doubles = new List<double>();

            doubles.Add(probability1);
            doubles.Add(probability2);
            doubles.Add(probability3);
            doubles.Add(probability4);
            doubles.Add(probability5);
            doubles.Add(probability6);
            doubles.Add(probability7);
            doubles.Add(probability8);
            doubles.Add(probability9);
            doubles.Add(probability10);

            var list = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                double temp = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    temp += doubles[j];
                }

                list.Add(temp);
            }

            foreach (var VARIABLE in list)
            {
                Console.WriteLine(VARIABLE);
            }

            int next = new Random().Next(1, 10001);
            int num = 0;
            if (next <= list[0]) num = 1;
            else if (next <= list[1])
                num = 2;
            else if (next <= list[2])
                num = 3;
            else if (next <= list[3])
                num = 4;
            else if (next <= list[4])
                num = 5;
            else if (next <= list[5])
                num = 6;
            else if (next <= list[6])
                num = 7;
            else if (next <= list[7])
                num = 8;
            else if (next <= list[8])
                num = 9;
            else if (next <= list[9])
                num = 10;
            Console.WriteLine(next);
            Console.WriteLine(num);
            return num;
        }
    }
}