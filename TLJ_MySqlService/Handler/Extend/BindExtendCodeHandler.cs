using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NhInterMySQL.Manager;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_BindTuiGuangCode)]
    class BindExtendCodeHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            BindExtendCode bindExtendCode = null;
            try
            {
                bindExtendCode = JsonConvert.DeserializeObject<BindExtendCode>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误：" + e);
                return null;
            }

            string _tag = bindExtendCode.tag;
            int _connId = bindExtendCode.connId;
            string code = bindExtendCode.tuiguangcode;
            string uid = bindExtendCode.uid;

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(uid) ||
                string.IsNullOrWhiteSpace(code))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            BindCode(uid, code, _responseData);

            return _responseData.ToString();
        }

        private void BindCode(string uid, string code, JObject responseData)
        {
            UserInfo userInfo = MySqlManager<UserInfo>.Instance.GetUserInfoByCode(code);
            if (userInfo == null)
            {
                OperatorFail(responseData, "该推广码不存在");
            }
            else
            {
                UserExtend userExtend = MySqlManager<UserExtend>.Instance.GetByUid(uid);
                if (userExtend != null)
                {
                    OperatorFail(responseData, "您已绑定推广码");
                }
                else
                {
                    if (userInfo.Uid == uid)
                    {
                        OperatorFail(responseData, "不能绑定自己的推广码");
                        return;
                    }

                    userExtend = new UserExtend()
                    {
                        Uid = uid,
                        extend_uid = userInfo.Uid,
                        task1 = 1,
                        task2 = 1
                    };

                    if (MySqlManager<UserExtend>.Instance.Add(userExtend))
                    {
                        responseData.Add("reward", "121:1");
                        MySqlUtil.AddProp(uid, "121:1", "绑定推广码奖励");
                        OperatorSuccess(responseData, "绑定成功");
                    }
                    else
                    {
                        OperatorFail(responseData, "添加数据失败");
                    }
                }
            }
        }

        private void OperatorSuccess(JObject responseData,string msg)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("msg", msg);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData,string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}