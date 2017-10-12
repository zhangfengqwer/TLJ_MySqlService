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
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class UsePropHandler : BaseHandler
    {
        private static MySqlManager<UserProp> userPropManager = new MySqlManager<UserProp>();

        public UsePropHandler()
        {
            tag = Consts.Tag_UseProp;
        }

        public override string OnResponse(string data)
        {
            UsePropReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UsePropReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;
            int propId = defaultReqData.prop_id;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            UsePropSql(Uid, propId, _responseData);
            return _responseData.ToString();
        }

        private void UsePropSql(string uid, int propId, JObject responseData)
        {
            UserProp userProp = userPropManager.GetUserProp(uid, propId);
            if (userProp == null || userProp.PropNum <= 0 || userProp.Type != 0)
            {
                MySqlService.log.Warn("没有该道具或者不能使用该道具");
                OperatorFail(responseData);
            }
            else
            {
                userProp.PropNum--;
                if (userPropManager.Update(userProp))
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