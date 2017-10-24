using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            GetRankListSql(responseData);
            return responseData.ToString();
        }

        /// <summary>
        /// 查询排行数据库
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void GetRankListSql(JObject responseData)
        {
            List<UserInfo> orderByJinbi = userInfoManager.GetListOrderByLimit(30) as List<UserInfo>;
            List<JinbiRankJsonObject> JinbiRanks = new List<JinbiRankJsonObject>();
            if (orderByJinbi ==null) orderByJinbi = new List<UserInfo>();
            for (int i = 0; i < orderByJinbi.Count; i++)
            {
                JinbiRankJsonObject jinbiRankJsonObject = new JinbiRankJsonObject();
                jinbiRankJsonObject.name = orderByJinbi[i].NickName;
                jinbiRankJsonObject.gold = orderByJinbi[i].Gold;
                jinbiRankJsonObject.head = orderByJinbi[i].Head;
                JinbiRanks.Add(jinbiRankJsonObject);
            }
            OperatorSuccess(JinbiRanks, responseData);
        }


        //数据库操作成功
        private void OperatorSuccess(List<JinbiRankJsonObject> jinbiRanks, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("jinbi_list", JsonConvert.SerializeObject(jinbiRanks));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}