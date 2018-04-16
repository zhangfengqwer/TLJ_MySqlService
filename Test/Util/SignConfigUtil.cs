using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Test;

namespace Util
{
    public class SignConfigUtil
    {
        public static void ExportClass(string fileName, string exportDir)
        {
            XSSFWorkbook xssfWorkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xssfWorkbook = new XSSFWorkbook(file);
            }

            List<SignReward_30> signReward30S = new List<SignReward_30>();
            ISheet sheet = xssfWorkbook.GetSheetAt(1);
            for (int i = 1; i < 36; i++)
            {
                SignReward_30 signReward30 = new SignReward_30();
                signReward30.id = int.Parse(GetCellString(sheet, i, 0));
                signReward30.day = int.Parse(GetCellString(sheet, i, 0));
                if (i >= 32)
                {
                    signReward30.type = 2;
                }
                else
                {
                    signReward30.type = 1;
                }

                if (i == 32)
                {
                    signReward30.day = 3;
                }
                else if (i == 33)
                {
                    signReward30.day = 12;
                }
                else if (i == 34)
                {
                    signReward30.day = 21;
                }
                else if (i == 35)
                {
                    //下个月的天数
                    int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month + 1);
                    signReward30.day = daysInMonth;
                }

                signReward30.reward_name = GetCellString(sheet, i, 1);
                signReward30.reward_prop = GetCellString(sheet, i, 2);
                signReward30S.Add(signReward30);
            }

            using (FileStream fs = new FileStream(exportDir, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonConvert.SerializeObject(signReward30S));
            }
        }

        private static string GetCellString(ISheet sheet, int i, int j)
        {
            return sheet.GetRow(i)?.GetCell(j)?.ToString() ?? "";
        }
    }
}