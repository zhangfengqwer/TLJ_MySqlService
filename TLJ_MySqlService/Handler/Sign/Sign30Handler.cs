﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string signTag = defaultReq.tag;
            int signConnId = defaultReq.connId;
            string signUid = defaultReq.uid;
            int id = defaultReq.id;

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

            //签到
            Sign30Sql(signUid, _responseData);
            return _responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="signUid"></param>
        /// <param name="responseData"></param>
        private void Sign30Sql(string signUid, JObject responseData)
        {

            responseData.Add("reward_prop", Sign30Data.getInstance().getSign30DataContentList()[1].reward_prop);
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
//            Sign signByUid = NHibernateHelper.signManager.GetByName(signUid);
//            if (signByUid == null)
//            {
//                OperatorFail(responseData);
//                MySqlService.log.Warn("传入的uid有误");
//            }
//            else
//            {
//                DateTime updateTime = signByUid.UpdateTime;
//                int updateTimeYear = updateTime.Year;
//                int updateTimeMonth = updateTime.Month;
//                int updateTimeDay = updateTime.Day;
//                int nowYear = DateTime.Now.Year;
//                int nowMonth = DateTime.Now.Month;
//                int nowDay = DateTime.Now.Day;
//                //已经签到过了，不能签到
//                if (updateTimeYear == nowYear && updateTimeMonth == nowMonth && updateTimeDay == nowDay &&
//                    signByUid.SignWeekDays != 0)
//                {
//                    OperatorFail(responseData);
//                    MySqlService.log.Warn("已签到:" + signUid);
//                }
//                //未签到，可以签到
//                else
//                {
//                    SignConfig signConfig = MySqlService.SignConfigs[signByUid.SignWeekDays];
//                    signByUid.SignWeekDays++;
//                    signByUid.UpdateTime = DateTime.Now;
//                    MySqlService.log.Info(signConfig.goods_prop);
//
//                    string reason = "";
//                    if (signByUid.SignWeekDays == 7)
//                    {
//                        reason = "签到奖励_大礼包";
//                    }
//                    else
//                    {
//                        reason = "签到奖励";
//                    }
//
//                    if (NHibernateHelper.signManager.Update(signByUid) && MySqlUtil.AddProp(signUid, signConfig.goods_prop, reason))
//                    {
//                        OperatorSuccess(responseData);
//                    }
//                    else
//                    {
//                        OperatorFail(responseData);
//                    }
//                }
//            }
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