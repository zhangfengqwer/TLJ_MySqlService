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
    class GetUseNoticeHandler : BaseHandler
    {
        private MySqlManager<UserNotice> userNoticeManager = new MySqlManager<UserNotice>();
        private MySqlManager<Notice> noticeManager = new MySqlManager<Notice>();

        public GetUseNoticeHandler()
        {
            tag = Consts.Tag_GetNotice;
        }

        public override string OnResponse(string data)
        {
            DefaultReqData defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<DefaultReqData>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);
            //得到用户通知
            GetUseNoticeSql(Uid, _responseData);
            return _responseData.ToString();
        }

        private void GetUseNoticeSql(string uid,JObject responseData)
        {
            List<Notice> notices = noticeManager.GetAll().ToList();
            List<UserNotice> userNotices = userNoticeManager.GetListByUid(uid);
            List<UserNoticeJsonObj> tempList = new List<UserNoticeJsonObj>();

            if (notices.Count != 0)
            {
                if (userNotices.Count != notices.Count)
                {
                    for (int i = 0; i < notices.Count; i++)
                    {
                        Notice notice = notices[i];
                        UserNotice userNotice = new UserNotice()
                        {
                            Uid = uid,
                            NoticeId = notice.NoticeId,
                            State = 0
                        };
                        if (!userNoticeManager.Add(userNotice))
                        {
//                            OperatorFail(responseData);
                            MySqlService.log.Warn("添加用户通知错误");
//                            return;
                        }
                    }
                    userNotices = userNoticeManager.GetListByUid(uid);
                }
               
            }

            for (int i = 0; i < notices.Count; i++)
            {
                Notice notice = notices[i];
                for (int j = 0; j < userNotices.Count; j++)
                {
                    UserNotice userNotice = userNotices[j];
                    if (notice.NoticeId == userNotice.NoticeId)
                    {
                        UserNoticeJsonObj userNoticeJsonObj = new UserNoticeJsonObj()
                        {
                            notice_id = notice.NoticeId,
                            title = notice.Title,
                            content = notice.Content,
                            type = notice.Type,
                            state = userNotice.State,
                            start_time = notice.StartTime,
                            end_time = notice.EndTime
                        };
                        tempList.Add(userNoticeJsonObj);
                    }
                }
            }
            OperatorSuccess(tempList, responseData);

        }

        //数据库操作成功
        private void OperatorSuccess(List<UserNoticeJsonObj> userNotices, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("notice_list", JsonConvert.SerializeObject(userNotices));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}