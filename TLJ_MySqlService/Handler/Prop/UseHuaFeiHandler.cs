using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJ_MySqlService.Model;
using TLJCommon;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class UseHuaFeiHandler : BaseHandler
    {
        public UseHuaFeiHandler()
        {
            Tag = Consts.Tag_UseHuaFei;
        }

        public override string OnResponse(string data) 
        {
            UseHuaFeiReq defaultReqData;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UseHuaFeiReq>(data);
            }
            catch (Exception)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;
            int propId = defaultReqData.prop_id;
            string phone = defaultReqData.phone;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid) || string.IsNullOrWhiteSpace(phone))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            UseHuaFeiSql(Uid, propId, _responseData);
            return _responseData.ToString();
        }

        private void UseHuaFeiSql(string uid, int propId, JObject responseData)
        {
            UserProp userProp = MySqlService.userPropManager.GetUserProp(uid, propId);
            if (userProp == null || userProp.PropNum <= 0)
            {
                MySqlService.log.Warn("没有该道具或者不能使用该道具");
                OperatorFail(responseData);
            }
            else
            {
                userProp.PropNum--;
                switch (userProp.PropId)
                {
                    //1元话费
                    case 111:
                        break;
                    //5元话费
                    case 112:
                        break;
                }
                if (MySqlService.userPropManager.Update(userProp))
                {
                    OperatorSuccess(responseData);
                }
                else
                {
                    MySqlService.log.Warn("使用道具失败");
                    OperatorFail(responseData);
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