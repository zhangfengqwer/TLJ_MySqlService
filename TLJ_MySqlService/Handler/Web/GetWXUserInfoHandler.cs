using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_WeChat_UserInfo)]
    class GetWXUserInfoHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag)
                || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            responseData.Add(MyCommon.UID, uid);
            GetUserInfoSql(uid, responseData);
            return responseData.ToString();
        }

        private void GetUserInfoSql(string uid, JObject responseData)
        {
            User user = NHibernateHelper.userManager.GetByUid(uid);

            if (user == null)
            {
                OperatorFail(responseData);
                MySqlService.log.Warn("传入的uid未注册");
            }
            else
            {
                UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
                if (userInfo != null)
                {
                    OperatorSuccess(userInfo, responseData);
                }
                else
                {
                    MySqlService.log.Warn("用户信息表中没有该uid信息：" + uid);
                    OperatorFail(responseData);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(UserInfo userInfo,JObject responseData)
        {
          
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("name", userInfo.NickName);
            responseData.Add("huizhangNum", userInfo.Medel);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}