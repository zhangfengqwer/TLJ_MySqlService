using NhInterMySQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Model;

namespace Test
{
    class Program
    {
        private static HashSet<string> hashSet = new HashSet<string>();
        private static List<String> xingList = new List<string>();
        private static List<String> maleList = new List<string>();
        private static List<String> femaleList = new List<string>();
//        public static Dictionary<TljServiceType, IntPtr> serviceDic = new Dictionary<TljServiceType, IntPtr>();
        static void Main(string[] args)
        {

            //            var md5 = GetMD5("123");
            //            string changePsw = ChangePsw("123");
            //            var checkPsw = CheckPsw("202CB962AC5975B964B7152D234B70");

            //            Console.WriteLine(checkPsw);
            //            Console.WriteLine(checkPsw);

            //            foreach (var user in NHibernateHelper.userNoticeManager.GetAll())
            //            {
            //                if (user.NoticeId == 4)
            //                {
            //                    NHibernateHelper.userNoticeManager.Delete(user);
            //                }
            //            }
            //            var user = new User();
            //            string uid = UidUtil.createUID();
            //            user.Uid = uid;
            //            user.Platform = 0;
            //            user.ThirdId = "";
            //            user.Secondpassword = "";
            //            user.IsRobot = 0;
            //            user.Userpassword = "C3981FA8D26E95D911FE8EAEB657F2F";
            //            NHibernateHelper.userManager.Add(user);
            //         
            //            string changePsw = ChangePsw("lq3445www");
            //            Console.WriteLine(changePsw);
            //            var stopwatch = new Stopwatch();
            //            stopwatch.Start();
            //            NHibernateHelper.userManager.VerifyLogin("123", "123");
            //            stopwatch.Stop();
            //            Console.WriteLine(stopwatch.ElapsedMilliseconds+"ms");
            //
            //            stopwatch.Restart();
            //            var sql = $"select username, userpassword from user where username = '123' and userpassword = '123' ";
            //
            //            using (var session = NHibernateHelper.OpenSession())
            //            {
            //                using (var transaction = session.BeginTransaction())
            //                {
            //                    var user = session.CreateSQLQuery(sql).UniqueResult();
            //                    var user1 = user as User;
            //                    Console.WriteLine(user1);
            //
            //                }
            //            }
            //
            //            stopwatch.Stop();
            //            Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms");


            //            // 异步方法全部会回掉到主线程
            //            OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
            //            SynchronizationContext.SetSynchronizationContext(contex);
            //
            //            while (true)
            //            {
            //                try
            //                {
            //                    Thread.Sleep(1);
            //                    contex.Update();
            //                }
            //                catch (Exception e)
            //                {
            //                    Console.WriteLine(e);
            //                }
            //            }

            //            string directory = Environment.CurrentDirectory;
            //            for (int i = 0; i < 3; i++)
            //            {
            //                directory = CommonUtil.GetFrontDirectory(directory);
            //            }
            //           
            //
            //            string path = Path.Combine(directory, @"TLJ_MySqlService\Lib\test.txt");
            ////            int lastIndexOf = path.LastIndexOf('\\');
            ////            path = path.Remove(lastIndexOf);
            //
            //            Console.WriteLine(path);
            //            if (File.Exists(path))
            //            {
            //                using (FileStream fs = new FileStream(path, FileMode.Open))
            //                {
            //                    StreamReader reader = new StreamReader(fs);
            //                    var readLine = reader.ReadLine();
            //                    Console.WriteLine(readLine);
            //                }
            //            }
            //            else
            //            {
            //                Console.WriteLine("meiyou");
            //            }

            //            for (int i = 10001; i < 20001; i++)
            //            {
            //                ModelFactory.CreateUser(i+"");
            //            }
            //
            //
            //            Assembly assembly = typeof(MySqlService).Assembly;
            //            Console.WriteLine(assembly.FullName);

            //            UserTask userTask = NHibernateHelper.userTaskManager.GetUserTask("6509453643", 208);
            //            Console.Write(userTask.progress);

            //            foreach (var userTask in NHibernateHelper.userTaskManager.GetAll())
            //            {
            //                if (userTask.task_id == 219)
            //                {
            //                    NHibernateHelper.userTaskManager.Delete(userTask);
            //                }
            //
            //            }

            Console.ReadKey();
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
                    $"({ai.UserId},'{ai.Uid}','{ai.Username}','{ai.Userpassword}','{ai.Secondpassword}','{ai.ThirdId}','{ai.Platform}','{ai.IsRobot}'),";
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
//            dataGridView1.DataSource = dt; //绑定到界面
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
            else if (next <= list[1]) num = 2;
            else if (next <= list[2]) num = 3;
            else if (next <= list[3]) num = 4;
            else if (next <= list[4]) num = 5;
            else if (next <= list[5]) num = 6;
            else if (next <= list[6]) num = 7;
            else if (next <= list[7]) num = 8;
            else if (next <= list[8]) num = 9;
            else if (next <= list[9]) num = 10;
            Console.WriteLine(next);
            Console.WriteLine(num);
            return num;
        }
    }
}