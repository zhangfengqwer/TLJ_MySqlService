using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class RandomName
{
    private static List<string> xingList;
    private static List<string> maleList;
    private static List<string> femaleList;

    public static string GetName()
    {
        if (xingList == null || xingList.Count == 0)
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "RandomName.json");
            string str = sr.ReadToEnd().ToString();
            sr.Close();

            JObject jo = JObject.Parse(str);

            xingList = JsonConvert.DeserializeObject<List<String>>(jo["xing"].ToString());
            maleList = JsonConvert.DeserializeObject<List<String>>(jo["maleMing"].ToString());
            femaleList = JsonConvert.DeserializeObject<List<String>>(jo["femaleMing"].ToString());
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

    static int GetRandomSeed()
    {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng =
            new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
}