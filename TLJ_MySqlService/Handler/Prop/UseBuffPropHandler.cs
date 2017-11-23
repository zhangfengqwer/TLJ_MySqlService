using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NhInterMySQL;
using TLJ_MySqlService.Model;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    class UseBuffPropHandler : BaseHandler
    {
        public UseBuffPropHandler()
        {
            Tag = Consts.Tag_UseBuff;
        }

        public override string OnResponse(string data) 
        {
            UsePropReq defaultReqData;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UsePropReq>(data);
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

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add(MyCommon.UID, Uid);
            _responseData.Add("prop_id", propId);

            UsePropSql(Uid, propId, _responseData);
            return _responseData.ToString();
        }

        private void UsePropSql(string uid, int propId, JObject responseData)
        {
            UserProp userProp = NHibernateHelper.userPropManager.GetUserProp(uid, propId);
            if (userProp == null || userProp.BuffNum <= 0)
            {
                MySqlService.log.Warn("没有该道具或者不能使用该道具");
                OperatorFail(responseData);
            }
            else
            {
                userProp.BuffNum--;
                if (NHibernateHelper.userPropManager.Update(userProp))
                {
                    //TODO buff为0,删除玩家道具
                    OperatorSuccess(responseData);
                }
                else
                {
                    MySqlService.log.Warn("使用Buff道具失败");
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