﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetBag)]
    class GetUserBagHandler : BaseHandler
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
            int ConnId = defaultReq.connId;
            string Uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            GetUserBagSql(Uid, _responseData);
            return _responseData.ToString();
        }

        private void GetUserBagSql(string uid, JObject responseData)
        {
            try
            {
                List<UserProp> userProps = NHibernateHelper.userPropManager.GetListByUid(uid);
                List<UserPropJsonObject> tempList = new List<UserPropJsonObject>();
                UserPropJsonObject userPropJson;
                foreach (var prop in userProps)
                {
                    userPropJson = new UserPropJsonObject();
                    userPropJson.prop_id = prop.PropId;
                    userPropJson.prop_num = prop.PropNum;
                    //只有数量大于0的道具数据才会返回
                    if (userPropJson.prop_num > 0)
                    {
                        tempList.Add(userPropJson);
                    }
                }
                responseData.Add("prop_list", JsonConvert.SerializeObject(tempList));
                OperatorSuccess(responseData);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("查询道具失败:" + e);
                OperatorFail(responseData);
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