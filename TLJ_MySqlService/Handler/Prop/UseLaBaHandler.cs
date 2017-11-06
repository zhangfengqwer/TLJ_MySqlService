using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJ_MySqlService.Model;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class UseLaBaHandler : BaseHandler
    {
        public UseLaBaHandler()
        {
            Tag = Consts.Tag_UseLaBa;
        }

        public override string OnResponse(string data)
        {
            UseLaBaReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UseLaBaReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;
            string text = defaultReqData.text;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add("text", text);

            UseLaBaSql(Uid, _responseData);
            return _responseData.ToString();
        }

        private void UseLaBaSql(string uid, JObject responseData)
        {
            UserProp userProp = MySqlService.userPropManager.GetUserProp(uid, 106);
            if (userProp == null || userProp.PropNum <= 0)
            {
                MySqlService.log.Warn("没有喇叭");
                OperatorFail(responseData);
            }
            else
            {
                userProp.PropNum--;
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