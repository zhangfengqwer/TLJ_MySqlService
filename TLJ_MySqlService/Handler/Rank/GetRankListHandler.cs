﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class GetRankListHandler : BaseHandler
    {
        private MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();

        public GetRankListHandler()
        {
            tag = Consts.Tag_GetSignRecord;
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
                MySqlService.log.Warn("传入的参数有误:"+e);
                return null;
            }
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;
            string uid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(Tag) || connId == 0 || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            //查询
            GetRankListSql(uid, responseData);
            return responseData.ToString();
        }

        /// <summary>
        /// 查询签到数据库
        /// </summary>
        /// <param name="signUid"></param>
        /// <param name="responseData"></param>
        private void GetRankListSql(string signUid, JObject responseData)
        {
            IList<UserInfo> orderByJinbi = userInfoManager.getListOrderByJinbi();
            List<UserInfo> userInfos = orderByJinbi.Take(30).ToList();



        }


        //数据库操作成功
        private void OperatorSuccess(Sign sign, JObject responseData)
        {
            string updateTime = sign.UpdateTime.ToShortDateString();

            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.SIGNWEEKDAYS, sign.SignWeekDays);
            responseData.Add(MyCommon.UPDATETIME, updateTime);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}