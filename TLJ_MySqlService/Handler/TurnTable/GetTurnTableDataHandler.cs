using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TLJ_MySqlService.Model;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    class GetTurnTableDataHandler : BaseHandler
    {
       

        public GetTurnTableDataHandler()
        {
            Tag = Consts.Tag_GetTurntable;
        }

        public override string OnResponse(string data)
        {
            CommonReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<CommonReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;

            if (string.IsNullOrWhiteSpace(Tag))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);

            //得到pvp数据
            GetTurnTableDataSql(_responseData);
            return _responseData.ToString() ;
        }

        private void GetTurnTableDataSql(JObject responseData)
        {
            OperatorSuccess(MySqlService.TurnTables, responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(List<TurnTable> turnTables, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("turntable_list", JsonConvert.SerializeObject(turnTables));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
