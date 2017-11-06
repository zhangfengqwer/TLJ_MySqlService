using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService;
using TLJ_MySqlService.Handler;
using TLJ_MySqlService.JsonObject;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class JoinRoomHandler : BaseHandler
    {
        public JoinRoomHandler()
        {
            Tag = Consts.Tag_ReadNotice;
        }

        public override string OnResponse(string data)
        {
            JoinRoomReq joinRoomReq = null;
            try
            {
                joinRoomReq = JsonConvert.DeserializeObject<JoinRoomReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = joinRoomReq.tag;
            int ConnId = joinRoomReq.connId;
            string Uid = joinRoomReq.uid;
            string gameroomtype = joinRoomReq.gameroomtype;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid) || string.IsNullOrWhiteSpace(gameroomtype))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add("gameroomtype", gameroomtype);

            //加入房间
            JoinRoomSql(Uid, gameroomtype, _responseData);
            return _responseData.ToString();
        }

        private void JoinRoomSql(string uid, string gameroomtype, JObject responseData)
        {
            
        }

        //数据库操作成功
        private void OperatorSuccess( JObject responseData)
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