using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GameOver)]
    class GetGameOverHandler : BaseHandler
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
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            var gameroomtype = defaultReq.gameroomtype;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(gameroomtype))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);

            //得到pvp数据
            GetGameOverSql(uid, gameroomtype);
            return null;
        }

        private void GetGameOverSql(string uid, string gameroomtype)
        {
            var pvpGameRoom = NhInterMySQL.NHibernateHelper.PVPGameRoomManager.GetPVPRoom(gameroomtype);
            if (pvpGameRoom != null)
            {
                if ("0".Equals(pvpGameRoom.baomingfei)) return;

                var baomingfei = pvpGameRoom.baomingfei;
                var split = baomingfei.Split(':');
                var s1 = split[0];
                var s2 = split[1];
                string s3 = "-" + s2;
                baomingfei = s1 + ":" + s3;
                MySqlService.log.Info($"uid:{uid},报名费：{baomingfei}");

                MySqlUtil.ConfigExpenseGold(uid, Convert.ToInt32(s2));
//                MySqlUtil.AddProp(uid, baomingfei);
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