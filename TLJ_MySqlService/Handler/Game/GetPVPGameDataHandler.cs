using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class GetPVPGameDataHandler : BaseHandler
    {
       

        public GetPVPGameDataHandler()
        {
            tag = Consts.Tag_GetPVPGameRoom;
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
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;
            string uid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(Tag) || connId == 0 || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);

            //得到pvp数据
            GetPVPDataSql(_responseData);
            return _responseData.ToString() ;
        }

        private void GetPVPDataSql(JObject responseData)
        {
            OperatorSuccess(MySqlService.PvpGameRooms, responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(List<PVPGameRoom> pvpGameRooms, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("room_list", JsonConvert.SerializeObject(pvpGameRooms));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
