using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Manager;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_OneKeyGetAllTuiGuangReward)]
    class OneKeyGetAllTuiGuangRewardHandler : BaseHandler   
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
            string uid = bindExtendCode.uid;

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            GetAllTuiGuangReward(uid,  _responseData);

            return _responseData.ToString();
        }

        private void GetAllTuiGuangReward(string uid, JObject jObject)
        {
            List<UserExtend> userExtends = MySqlManager<UserExtend>.Instance.GetMyExtendDataByUid(uid).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var userExtend in userExtends)
            {
                if (userExtend.task1 == 2)
                {
                    MySqlUtil.AddProp(uid,"121:1",$"领取{userExtend.Uid}的任务1奖励");
                    sb.Append("121:1");
                    sb.Append(";");
                    userExtend.task1 = 3;
                }

                if (userExtend.task2 == 2)
                {
                    MySqlUtil.AddProp(uid, "111:1", $"领取{userExtend.Uid}的任务2奖励");
                    sb.Append("111:1");
                    sb.Append(";");
                    userExtend.task2 = 3;
                }
                MySqlManager<UserExtend>.Instance.Update(userExtend);
            }

            jObject.Add("reward", sb.ToString());
            jObject.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
        }
    }
}