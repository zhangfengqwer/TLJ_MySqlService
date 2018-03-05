using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJ_MySqlService.Utils;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_Sign_30)]
    class Sign30Handler : BaseHandler
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

            string signTag = defaultReq.tag;
            int signConnId = defaultReq.connId;
            string signUid = defaultReq.uid;
            int id = defaultReq.id;
            int type = defaultReq.type;

            if (string.IsNullOrWhiteSpace(signTag) || string.IsNullOrWhiteSpace(signUid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, signTag);
            _responseData.Add(MyCommon.CONNID, signConnId);
            _responseData.Add("id", id);
            _responseData.Add("type", type);

            //签到
            Sign30Sql(signUid, id, type, _responseData);
            return _responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="id"></param>
        /// <param name="signType"></param>
        /// <param name="responseData"></param>
        private void Sign30Sql(string uid, int id, int signType, JObject responseData)
        {
            Sign30DataContent dataContent = Sign30Data.getInstance().getSign30DataById(id);

            switch (signType)
            {
                case 1:
                case 2:
                    sign(uid, dataContent, responseData);
                    break;
                case 3:
                    totalSign(uid, dataContent, responseData);
                    break;
            }
        }

        private void totalSign(string uid, Sign30DataContent dataContent, JObject responseData)
        {
            List<UserMonthSign> userMonthSigns = GetSign30RecordHandler.GetSign30RecordSql(uid);
            int dataContentDay = dataContent.day;

            for (int i = userMonthSigns.Count - 1; i <= 0; i++)
            {
                if (int.Parse(userMonthSigns[i].SignDate) > 31)
                {
                    userMonthSigns.RemoveAt(i);
                }
            }

            if (userMonthSigns.Count >= dataContentDay)
            {
                UserMonthSign userMonthSign = new UserMonthSign()
                {
                    Uid = uid,
                    SignYearMonth = GetSign30RecordHandler.GetYearMonth(),
                    SignDate = dataContent.id + ""
                };

                if (MySqlManager<UserMonthSign>.Instance.Add(userMonthSign))
                {
                    responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
                    responseData.Add("msg", $"{dataContent.reward_name}领取成功");
                    responseData.Add("reward_prop", dataContent.reward_prop);
                    AddSignReward(uid, dataContent.reward_prop);
                }
                else
                {
                    responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                    responseData.Add("msg", $"{dataContent.reward_name}的奖励已领取,不可重复领取");
                }
            }
            else
            {
                responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
                responseData.Add("msg", $"未满足{dataContent.reward_name},当前签到累计{userMonthSigns.Count}");
            }
        }

        private void sign(string uid, Sign30DataContent dataContent, JObject responseData)
        {
            int dataContentDay = dataContent.day;
            int nowYear = DateTime.Now.Year;
            int nowMonth = DateTime.Now.Month;
            string signYearMonth = $"{nowYear}-{nowMonth}";
            UserMonthSign userMonthSign = MySqlManager<UserMonthSign>.Instance.GetUserMonthSign(uid, signYearMonth, dataContentDay + "");
            if (userMonthSign == null)
            {
                userMonthSign = new UserMonthSign()
                {
                    Uid = uid,
                    SignDate = dataContentDay + "",
                    SignYearMonth = signYearMonth
                };

                if (MySqlManager<UserMonthSign>.Instance.Add(userMonthSign))
                {
                    responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
                    responseData.Add("reward_prop", dataContent.reward_prop);
                    responseData.Add("msg", "签到成功");
                    AddSignReward(uid, dataContent.reward_prop);
                }
                else
                {
                    responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
                    responseData.Add("msg", "已签到");
                }
            }
            else
            {
                responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
                responseData.Add("msg", "已签到");
            }
        }

        private void AddSignReward(string uid, string dataContentRewardProp)
        {
            MySqlUtil.AddProp(uid, dataContentRewardProp, "每月签到奖励");
        }
    }
}