using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetSignRecord)]
    class GetSignRecordHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误:"+e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag)  || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            NHibernateHelper.signManager = new MySqlManager<Sign>();
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            //查询
            GetSignDataSql(uid, responseData);
            return responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="signUid"></param>
        /// <param name="responseData"></param>
        private void GetSignDataSql(string signUid, JObject responseData)
        {
            Sign sign = NHibernateHelper.signManager.GetByName(signUid);
            //签到表中没有用户信息
            if (sign == null)
            {
                //注册签到数据
                sign = new Sign()
                {
                    Uid = signUid,
                    SignWeekDays = 0,
                    UpdateTime = DateTime.Now
                };
                if (NHibernateHelper.signManager.Add(sign))
                {
                    OperatorSuccess(sign, responseData);
                }
                else
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn("添加签到数据数据失败");
                }
              
            }
            else
            {
                OperatorSuccess(sign, responseData);
            }
        }


        //数据库操作成功
        private void OperatorSuccess(Sign sign, JObject responseData)
        {
            string updateTime = sign.UpdateTime.ToShortDateString();

            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.SIGNWEEKDAYS, sign.SignWeekDays);
            responseData.Add(MyCommon.UPDATETIME, updateTime);
            responseData.Add("sign_config", JsonConvert.SerializeObject(MySqlService.SignConfigs));


        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}