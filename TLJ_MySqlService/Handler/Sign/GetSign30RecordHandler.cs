using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJCommon;
using NhInterMySQL.Model;
using NHibernate;
using NHibernate.Criterion;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetSignRecord_30)]
    class GetSign30RecordHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            DefaultReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<DefaultReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }

            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);

            List<UserMonthSign> userMonthSigns = GetSign30RecordSql(uid);
            int num = 0;
            if (userMonthSigns.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = userMonthSigns.Count - 1; i >= 0; i--)
                {
                    if (userMonthSigns[i].Type == 2)
                    {
                        userMonthSigns.RemoveAt(i);
                        num++;
                    }
                }
                foreach (var sign in userMonthSigns)
                {
                    string signDate = sign.SignDate;
                    sb.Append(signDate);
                    sb.Append(",");
                }
                responseData.Add("record", sb.ToString().Remove(sb.ToString().Length - 1));
            }
            else
            {
                responseData.Add("record", "");
            }

            responseData.Add("curMonthBuQianCount", num);

            return responseData.ToString();
        }

        public static List<UserMonthSign> GetSign30RecordSql(string uid)
        {
            var yearMonth = GetYearMonth();
            var sql = $"SELECT * FROM new_tlj.user_month_sign where uid = '{uid}' and sign_year_month = '{yearMonth}'";
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        List<UserMonthSign> userMonthSigns = session.CreateSQLQuery(sql).AddEntity(typeof(UserMonthSign)).List<UserMonthSign>().ToList();
                        transaction.Commit();
                        return userMonthSigns;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }

        public static string GetYearMonth()
        {
            int nowYear = DateTime.Now.Year;
            int nowMonth = DateTime.Now.Month;
            string yearMonth = $"{nowYear}-{nowMonth}";
            return yearMonth;
        }
    }
}