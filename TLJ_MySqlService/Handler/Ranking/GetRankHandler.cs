using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJ_MySqlService.Model;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    class GetRankHandler : BaseHandler
    {

        public GetRankHandler()
        {
            Tag = Consts.Tag_GetRank;
        }

        public override string OnResponse(string data)
        {
            CommonReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<CommonReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:"+e);
                return null;
            }
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;

            if (string.IsNullOrWhiteSpace(Tag) )
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            //查询
            GetRankSql(responseData);
            return responseData.ToString();
        }

        /// <summary>
        /// 查询排行数据库
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void GetRankSql(JObject responseData)
        {
            //金币榜
            List<UserInfo> orderByJinbi = NHibernateHelper.userInfoManager.GetGoldRank(30) as List<UserInfo>;
            List<GoldRankJsonObject> JinbiRanks = new List<GoldRankJsonObject>();
            if (orderByJinbi ==null) orderByJinbi = new List<UserInfo>();
            for (int i = 0; i < orderByJinbi.Count; i++)
            {
                GoldRankJsonObject jinbiRankJsonObject = new GoldRankJsonObject();
                jinbiRankJsonObject.name = orderByJinbi[i].NickName;
                jinbiRankJsonObject.gold = orderByJinbi[i].Gold;
                jinbiRankJsonObject.head = orderByJinbi[i].Head;
                JinbiRanks.Add(jinbiRankJsonObject);
            }

            //奖章榜
            List<UserInfo> orderByMedal = NHibernateHelper.userInfoManager.GetMedalRank(30) as List<UserInfo>;
            List<MedalRankJsonObject> medalRanks = new List<MedalRankJsonObject>();
            if (orderByMedal == null) orderByMedal = new List<UserInfo>();
            for (int i = 0; i < orderByMedal.Count; i++)
            {
                MedalRankJsonObject medalRankJsonObject = new MedalRankJsonObject();
                medalRankJsonObject.name = orderByMedal[i].NickName;
                medalRankJsonObject.medal = orderByMedal[i].Medel;
                medalRankJsonObject.head = orderByMedal[i].Head;
                medalRanks.Add(medalRankJsonObject);
            }
            OperatorSuccess(JinbiRanks,medalRanks, responseData);
        }

       

        //数据库操作成功
        private void OperatorSuccess(List<GoldRankJsonObject> jinbiRanks, List<MedalRankJsonObject> medalRanks, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("gold_list", JsonConvert.SerializeObject(jinbiRanks));
            responseData.Add("medal_list", JsonConvert.SerializeObject(medalRanks));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}