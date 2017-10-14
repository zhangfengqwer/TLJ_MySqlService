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
using TLJ_MySqlService.JsonObject;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class ReadNoticeHandler : BaseHandler
    {
        private MySqlManager<UserNotice> userNoticeManager = new MySqlManager<UserNotice>();

        public ReadNoticeHandler()
        {
            tag = Consts.Tag_ReadNotice;
        }

        public override string OnResponse(string data)
        {
            ReadNoticeReq readNoticeReq = null;
            try
            {
                readNoticeReq = JsonConvert.DeserializeObject<ReadNoticeReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = readNoticeReq.tag;
            int ConnId = readNoticeReq.connId;
            string Uid = readNoticeReq.uid;
            int noticeId = readNoticeReq.notice_id;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            _responseData.Add("notice_id", noticeId);
            //得到用户通知
            ReadNoticeSql(Uid, noticeId, _responseData);
            return _responseData.ToString();
        }

        private void ReadNoticeSql(string uid, int noticeId, JObject responseData)
        {
            UserNotice userNotice = userNoticeManager.getUserNotice(uid, noticeId);
            if (userNotice == null)
            {
                MySqlService.log.Warn("该用户没有这条通知");
                OperatorFail(responseData);
            }
            else
            {
                if (userNotice.State == 0)
                {
                    userNotice.State = 1;
                    if (userNoticeManager.Update(userNotice))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        MySqlService.log.Warn("更新用户通知数据库失败");
                        OperatorFail(responseData);
                    }
                }
                else
                {
                    MySqlService.log.Warn("该通知已读过");
                    OperatorFail(responseData);
                }
                
            }
        }

        //数据库操作成功
        private void OperatorSuccess( JObject responseData)
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