using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NhInterMySQL.Manager;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetMedalDuiHuanReward)]
    class GetMedalDuiHuanRewardHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            BindExtendCode bindExtendCode = null;
            try
            {
                bindExtendCode = JsonConvert.DeserializeObject<BindExtendCode>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误：" + e);
                return null;
            }

            string _tag = bindExtendCode.tag;
            int _connId = bindExtendCode.connId;

            if (string.IsNullOrWhiteSpace(_tag))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            _responseData.Add("medalDuiHuanRewardDataList", JsonConvert.SerializeObject(MySqlService.medalExchargeRewardDatas));

            return _responseData.ToString();
        }
    }
}