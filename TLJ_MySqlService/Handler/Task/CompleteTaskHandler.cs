using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService.Model;
using Zfstu.Manager;
using Zfstu.Model;
using Task = Zfstu.Model.Task;

namespace TLJ_MySqlService.Handler
{
    class CompleteTaskHandler : BaseHandler
    {
        private MySqlManager<Task> taskManager = new MySqlManager<Task>();
        private MySqlManager<UserTask> userTaskManager = new MySqlManager<UserTask>();
        private MySqlManager<UserProp> userPropManager = new MySqlManager<UserProp>();

        public CompleteTaskHandler()
        {
            tag = Consts.Tag_CompleteTask;
        }

        public override string OnResponse(string data)
        {
            CompleteReq completeReq = null;
            try
            {
                completeReq = JsonConvert.DeserializeObject<CompleteReq>(data);
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
            if (string.IsNullOrWhiteSpace(Tag) || connId == 0 || string.IsNullOrWhiteSpace(uid) || task_id < 0)
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
            UserTask userTask = userTaskManager.GetUserTask(uid, task_id);
            if (userTask == null)
            {
                MySqlService.log.Warn("没有该用户的任务");
                OperatorFail(responseData);
            }
            else
            {
                if (userTask.isover == 0 )
                {
                    userTask.isover = 1;
                    Task task = taskManager.GetTask(task_id);

                    if (userTaskManager.Update(userTask))
                    {
                        OperatorSuccess(responseData);
                    }
                    else
                    {
                        MySqlService.log.Warn("更新用户任务失败");
                        OperatorFail(responseData);
                    }
//                    AddProp(task, uid);
                    MySqlService.log.Info("奖励："+ task.reward);
                }
                else
                {
                    MySqlService.log.Warn("任务已完成");
                    OperatorFail(responseData);
                }
            }
        }

        private bool AddProp(Task task, string uid)
        {
            List<string> list = new List<String>();
            CommonUtil.splitStr(task.reward, list, ';');
            for (int i = 0; i < list.Count; i++)
            {
                string[] strings = list[i].Split(':');
                int propId = Convert.ToInt32(strings[0]);
                int propNum = Convert.ToInt32(strings[1]);

                UserProp userProp = userPropManager.GetUserProp(uid, propId);
                if (userProp == null)
                {
                    userProp = new UserProp()
                    {
                        Uid = uid,
                        PropId = propId,
                        PropNum = propNum
                    };
                    if (!userPropManager.Add(userProp))
                    {
                        MySqlService.log.Warn("添加道具失败");
                        return false;
                    }
                }
                else
                {
                    userProp.PropNum += propNum;
                    if (!userPropManager.Update(userProp))
                    {
                        MySqlService.log.Warn("更新道具失败");
                        return false;
                    }
                }
            }
            return true;
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
