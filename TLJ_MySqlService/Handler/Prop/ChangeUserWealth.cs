﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    class ChangeUserWealth : BaseHandler
    {
        public ChangeUserWealth()
        {
            Tag = Consts.Tag_ChangeUserWealth;
        }

        public override string OnResponse(string data) 
        {
            AddPropReq defaultReqData;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<AddPropReq>(data);
            }
            catch (Exception)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReqData.tag;
            string Uid = defaultReqData.uid;
            string account = defaultReqData.account;
            string password = defaultReqData.password;
            int reward_id = defaultReqData.reward_id;
            int reward_num = defaultReqData.reward_num;
            string connId = defaultReqData.connId;

            if (!MySqlService.AdminAccount.Equals(account) || !MySqlService.AdminPassWord.Equals(password))
            {
                MySqlService.log.Warn("账号错误");
                return null;
            }

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid) || reward_num == 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);

            AddPropSql(Uid, reward_id, reward_num, _responseData);
            return _responseData.ToString();
        }

        private void AddPropSql(string uid, int propId, int num, JObject responseData)
        {
            if (MySqlUtil.ChangeProp(uid, propId, num))
            {
                OperatorSuccess(responseData);
            }
            else
            {
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