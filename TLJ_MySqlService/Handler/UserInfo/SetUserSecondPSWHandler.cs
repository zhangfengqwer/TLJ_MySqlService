using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_SetSecondPSW)]
    class SetUserSecondPSWHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            SetUserSecondPSWReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<SetUserSecondPSWReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            string password = defaultReq.password;

            if (string.IsNullOrWhiteSpace(Tag)
                || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(password))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            SetUserSecondPSWSql(uid, password, responseData);
            return responseData.ToString();
        }

        private void SetUserSecondPSWSql(string uid, string password, JObject responseData)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn($"传入的uid未注册:{uid}");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Secondpassword))
                {
                    //设置二级密码
                    user.Secondpassword = password;
                    if (NHibernateHelper.userManager.Update(user))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        OperatorFail(responseData);
                        MySqlService.log.Warn($"更新二级密码失败-uid:{uid}-psw:{password}");
                    }
                }
                else
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn($"二级密码已经设置-uid:{uid}-psw:{password}");
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