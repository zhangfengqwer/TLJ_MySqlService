using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using TLJ_MySqlService.Utils;
using TLJCommon;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_ReadMail)]
    class ReadEmailHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            ReadEmailReq readEmailReq = null;
            try
            {
                readEmailReq = JsonConvert.DeserializeObject<ReadEmailReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = readEmailReq.tag;
            int connId = readEmailReq.connId;
            string uid = readEmailReq.uid;
            int emailId = readEmailReq.email_id;
            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || emailId < 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add(MyCommon.EMAIL_ID, emailId);

            //读取邮件
            ReadEmailSql(emailId, uid, _responseData);
            return _responseData.ToString();
        }

        private void ReadEmailSql(int emailId, string uid, JObject responseData)
        {
            UserEmail userEmail = NHibernateHelper.userEmailManager.GetEmail(emailId, uid);
            if (userEmail == null)
            {
                MySqlService.log.Warn("该邮件不存在");
                OperatorFail(responseData);
            }
            else
            {
                //未读
                if (userEmail.State == 0)
                {
                    userEmail.State = 1;
                    if (!string.IsNullOrWhiteSpace(userEmail.Reward))
                    {
                        bool addProp = MySqlUtil.AddProp(uid, userEmail.Reward, "领取邮件奖励");
                        if (!addProp)
                        {
                            MySqlService.log.Warn("读邮件加道具失败：" + uid + " " + userEmail.Reward);
                        }
                    }
                    if (NHibernateHelper.userEmailManager.Update(userEmail))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        MySqlService.log.Warn("读邮件失败");
                        OperatorFail(responseData);
                    }
                }
                else
                {
                    MySqlService.log.Warn("该邮件已读过");
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