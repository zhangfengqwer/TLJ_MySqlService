using NhInterMySQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test
{
    class Program
    {
        private static HashSet<string> hashSet = new HashSet<string>();
        private static List<String> xingList = new List<string>();
        private static List<String> maleList = new List<string>();
        private static List<String> femaleList = new List<string>();

        static void Main(string[] args)
        {
            string name = RandomName.GetName();
            string name1 = RandomName.GetName();
            string name2 = RandomName.GetName();
            //            Console.WriteLine(sb.ToString());
            Console.ReadKey();
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