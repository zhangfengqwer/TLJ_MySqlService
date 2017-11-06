using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJ_MySqlService.Utils;
using TLJCommon;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class SignHandler : BaseHandler
    {
        public SignHandler()
        {
            Tag = Consts.Tag_Sign;
        }

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

            if (string.IsNullOrWhiteSpace(signTag) 
                || string.IsNullOrWhiteSpace(signUid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, signTag);
            _responseData.Add(MyCommon.CONNID, signConnId);

            //签到
            SignSql(signUid, _responseData);
            return _responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="signUid"></param>
        /// <param name="responseData"></param>
        private void SignSql(string signUid, JObject responseData)
        {
            Sign signByUid = MySqlService.signManager.GetByName(signUid);
            if (signByUid == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid有误");
            }
            else
            {
                DateTime updateTime = signByUid.UpdateTime;
                int updateTimeYear = updateTime.Year;
                int updateTimeMonth = updateTime.Month;
                int updateTimeDay = updateTime.Day;
                int nowYear = DateTime.Now.Year;
                int nowMonth = DateTime.Now.Month;
                int nowDay = DateTime.Now.Day;
                //已经签到过了，不能签到
                if (updateTimeYear == nowYear && updateTimeMonth == nowMonth && updateTimeDay == nowDay &&
                    signByUid.SignWeekDays != 0)
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn("已签到");
                }
                //未签到，可以签到
                else
                {
                    SignConfig signConfig = MySqlService.SignConfigs[signByUid.SignWeekDays];
                    signByUid.SignWeekDays++;
                    signByUid.UpdateTime = DateTime.Now;
                    MySqlService.log.Info(signConfig.goods_prop);
                    if (MySqlService.signManager.Update(signByUid) && MySqlUtil.AddProp(signUid,signConfig.goods_prop))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        OperatorFail(responseData);
                    }
                }
            }
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