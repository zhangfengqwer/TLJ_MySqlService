using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NhInterMySQL;
using TLJ_MySqlService.Model;
using TLJ_MySqlService.Utils;
using TLJCommon;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_CompleteTask)]
    class CompleteTaskHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            CompleteTaskReq completeReq = null;
            try
            {
                completeReq = JsonConvert.DeserializeObject<CompleteTaskReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = completeReq.tag;
            int connId = completeReq.connId;
            string uid = completeReq.uid;
            int task_id = completeReq.task_id;
            if (string.IsNullOrWhiteSpace(Tag)  || string.IsNullOrWhiteSpace(uid) || task_id < 0)
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add("task_id", task_id);

            //读取邮件
            CompleteTaskSql(task_id, uid,_responseData);
            return _responseData.ToString() ;
        }

        private void CompleteTaskSql(int task_id, string uid, JObject responseData)
        {
            UserTask userTask = NHibernateHelper.userTaskManager.GetUserTask(uid, task_id);
            if (userTask == null)
            {
                var user = NHibernateHelper.userManager.GetByUid(uid);
                if (user.IsRobot != 1)
                {
                    MySqlService.log.Warn($"没有该用户的任务:{uid}");
                    OperatorFail(responseData);
                }
            }
            else
            {
                if (userTask.isover == 0 )
                {
                    Task task = NHibernateHelper.taskManager.GetTask(task_id);
                    if (userTask.progress == task.target)
                    {
                        userTask.isover = 1;
                        if (NHibernateHelper.userTaskManager.Update(userTask) && MySqlUtil.AddProp(uid, task.reward,"领取任务"))
                        {
                            OperatorSuccess(responseData);
                        }
                        else
                        {
                            MySqlService.log.Warn("更新任务奖励数据库失败");
                            OperatorFail(responseData);
                        }
                    }
                    else
                    {
                        MySqlService.log.Warn("任务未完成");
                        OperatorFail(responseData);
                    }
                }
                else
                {
                    MySqlService.log.Warn("任务奖励已领取");
                    OperatorFail(responseData);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}
