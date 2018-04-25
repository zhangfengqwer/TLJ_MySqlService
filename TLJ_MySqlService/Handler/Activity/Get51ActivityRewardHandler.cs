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
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler("Activity_51_GetReward")]
    class Get51ActivityRewardHandler : BaseHandler
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

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || id == 0)
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add("id", id);

            //得到用户51签到数据
            Get51ActivityRewardSql(uid, id,_responseData);
            return _responseData.ToString();
        }

        private void Get51ActivityRewardSql(string uid, int id, JObject responseData)
        {
            List<string> loginLists = new List<string>();

            for (int i = 0; i < MySqlService.activity51Datas.Count; i++)
            {
                Activity51Data activity51Data = MySqlService.activity51Datas[i];
                List<Log_Login> loginByDate = MySqlManager<Log_Login>.Instance.GetLoginByDate(uid, activity51Data.Time);
                //当天登陆过
                if (loginByDate.Count > 0)
                {
                    loginLists.Add(activity51Data.Time);
                }
            }

            MySqlService.log.Warn($"id:{id},count:{loginLists.Count}");

            if (id > loginLists.Count)
            {
                OperatorFail(responseData, "未达到天数要求");
            }
            else
            {
                if (MySqlManager<UserActivity51>.Instance.Add(new UserActivity51()
                {
                    Uid = uid,
                    activity_id = id
                }))
                {
                    Activity51Data activity51Data = new Activity51Data();
                    foreach (var item in MySqlService.activity51Datas)
                    {
                        if (item.Id == id)
                        {
                            activity51Data = item;
                            break;
                        }
                    }

                    MySqlUtil.AddProp(uid, activity51Data.Reward, "51活动");
                    OperatorSuccess(responseData, activity51Data.Reward);
                }
                else
                {
                    OperatorFail(responseData, "奖励已领取");
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData,string reward)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("reward", reward);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData,String msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}   