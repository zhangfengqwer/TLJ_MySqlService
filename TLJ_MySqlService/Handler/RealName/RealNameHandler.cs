using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TLJCommon;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class RealNameHandler : BaseHandler
    {
        public RealNameHandler()
        {
            tag = Consts.Tag_RealName;
        }

        public override string OnResponse(string data)
        {
            RealNameReq defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<RealNameReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReqData.tag;
            int connId = defaultReqData.connId;
            string uid = defaultReqData.uid;
            string identification = defaultReqData.identification;
            string realName = defaultReqData.realName;

            if (string.IsNullOrWhiteSpace(Tag) || connId == 0
                || string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(identification) 
                || string.IsNullOrWhiteSpace(realName))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            //实名认证
            UserRealNameSql(uid, realName, identification ,responseData);
            return responseData.ToString();
        }
        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="realName"></param>
        /// <param name="identification"></param>
        /// <param name="responseData"></param>
        private void UserRealNameSql(string uid, string realName, string identification, JObject responseData)
        {
            User user = MySqlService.userManager.GetByUid(uid);
            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserRealName userRealName = MySqlService.userRealNameManager.GetByUid(uid);
                if (userRealName == null)
                {
                    userRealName = new UserRealName();
                    userRealName.Uid = uid;
                    userRealName.RealName = realName;
                    userRealName.Identification = identification;
                    if (IDCardValidationUtil.CheckRealName(realName) && IDCardValidationUtil.CheckIDCard(identification))
                    {
                        if (MySqlService.userRealNameManager.Add(userRealName))
                        {
                            OperatorSuccess(responseData);
                        }
                        else
                        {
                            OperatorFail(responseData);
                            MySqlService.log.Warn("实名认证失败");
                        }
                    }
                    else
                    {
                        OperatorFail(responseData);
                        MySqlService.log.Warn("身份信息不正确");
                    }
                }
                else
                {
                    OperatorFail(responseData);
                    MySqlService.log.Warn("已实名");
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