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
    [Handler(Consts.Tag_ProgressTask)]
    public class ProgressTaskHandler : BaseHandler
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
            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || task_id < 0)
            {
                MySqlService.log.Warn($"字段有空:{data}");
                return null;
            }

            //进行任务
            ProgressTaskSql(task_id, uid);
            return null;
        }

        public static void ProgressTaskSql(int task_id, string uid)
        {
            UserTask userTask = NHibernateHelper.userTaskManager.GetUserTask(uid, task_id);
            if (userTask == null)
            {
                var user = NHibernateHelper.userManager.GetByUid(uid);
                if (user.IsRobot != 1)
                {
                    MySqlService.log.Warn($"没有该用户的任务{uid},task_id:{task_id}");
                }
            }
            else
            {
                if (userTask.isover == 0)
                {
                    Task task = NHibernateHelper.taskManager.GetTask(task_id);
                    if (task.target - userTask.progress >= 1)
                    {
                        //任务进度加一
                        userTask.progress++;
                        if (NHibernateHelper.userTaskManager.Update(userTask))
                        {
                            MySqlService.log.Info("更新任务成功");
                        }
                        else
                        {
                            MySqlService.log.Info("更新任务失败:" + uid + task_id);
                        }
                    }
                    else
                    {
                        MySqlService.log.Info("玩家任务已完成，请领取奖励:" + uid);
                    }
                }
                else
                {
                    MySqlService.log.Info("任务奖励已领取:" + uid);
                }
            }
        }
    }
}