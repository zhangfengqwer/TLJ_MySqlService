using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class SignHandler : BaseHandler
    {
        public SignHandler()
        {
            tag = Consts.Tag_Sign;
           
        }

        public override string OnResponse(string data)
        {
            DefaultReqData defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<DefaultReqData>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string signTag = defaultReqData.tag;
            int signConnId = defaultReqData.connId;
            string signUid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(signTag) || signConnId == 0
                || string.IsNullOrWhiteSpace(signUid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, signTag);
            _responseData.Add(MyCommon.CONNID, signConnId);
            //查询
            GetSignDataSql(signUid,_responseData);
            return _responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="signUid"></param>
        /// <param name="responseData"></param>
        private void GetSignDataSql(string signUid, JObject responseData)
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
                if (updateTimeYear == nowYear && updateTimeMonth == nowMonth && updateTimeDay == nowDay && signByUid.SignWeekDays != 0)
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn("已签到");
                }
                //未签到，可以签到
                else
                {
                    signByUid.SignWeekDays++;
                    signByUid.UpdateTime = DateTime.Now;
                    if (MySqlService.signManager.Update(signByUid))
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