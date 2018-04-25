using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NhInterMySQL.Manager;
using TLJ_MySqlService.JsonObject;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler("Activity_51_State")]
    class Get51ActivitySateHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReq.tag;
            int ConnId = defaultReq.connId;
            int id = defaultReq.id;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空:" + data);  
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            //得到用户51签到数据
            Get51ActivitySql(uid, _responseData);
            return _responseData.ToString();
        }

        private void Get51ActivitySql(string uid, JObject responseData)
        {
            List<string> loginLists = new List<string>();

            for (int i = 0; i < MySqlService.activity51Datas.Count; i++)
            {
                Activity51Data activity51Data = MySqlService.activity51Datas[i];
                List<Log_Login> loginByDate = MySqlManager<Log_Login>.Instance.GetLoginByDate(uid, activity51Data.Time);
                //当天登陆过
                if (loginByDate?.Count > 0)
                {
                    loginLists.Add(activity51Data.Time);
                }
            }

            JArray jArray = new JArray();

            for (int i = 0; i < MySqlService.activity51Datas.Count; i++)
            {
                Activity51Data activity51Data = MySqlService.activity51Datas[i];
                Activity51JsonObject activity51JsonObject = new Activity51JsonObject();
                activity51JsonObject.id = activity51Data.Id;
                activity51JsonObject.state = 3;

                if (activity51Data.Id <= loginLists.Count)
                {
                    List<UserActivity51> activity51s = MySqlManager<UserActivity51>.Instance.GetByPorpertyAndUid("activity_id", activity51Data.Id, uid);
                    //未领取
                    if (activity51s.Count == 0)
                    {
                        activity51JsonObject.state = 2;
                    }
                    //已领取
                    else
                    {
                        activity51JsonObject.state = 1;
                    }
                }

                JObject temp = new JObject();
                temp.Add("id", activity51JsonObject.id);
                temp.Add("state", activity51JsonObject.state);
               
                jArray.Add(temp);
            }

            responseData.Add("datalist", jArray);
            OperatorSuccess(responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}