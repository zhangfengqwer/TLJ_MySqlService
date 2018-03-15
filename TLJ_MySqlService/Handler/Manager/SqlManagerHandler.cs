using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_Execute_Sql)]
    class SqlManagerHandler : BaseHandler
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
            string _tag = defaultReq.tag;
            int _connId = defaultReq.connId;
            string sql = defaultReq.sql;

            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.CreateSQLQuery(sql).ExecuteUpdate();
                        transaction.Commit();
                        _responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        _responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                    }
                }
            }
            return _responseData.ToString();
        }
    }
}