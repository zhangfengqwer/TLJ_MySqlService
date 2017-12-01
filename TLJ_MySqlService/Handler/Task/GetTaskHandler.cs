using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using TLJCommon;
using NhInterMySQL.Model;
using Task = NhInterMySQL.Model.Task;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_GetTask)]
    class GetTaskHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误:"+e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject responseData = new JObject();
            responseData.Add(MyCommon.TAG, Tag);
            responseData.Add(MyCommon.CONNID, connId);
            //查询
            GetTaskSql(uid, responseData);
            return responseData.ToString();
        }

        private void GetTaskSql(string Uid, JObject responseData)
        {
            List<Task> tasks = NHibernateHelper.taskManager.GetAll().ToList();
            List<UserTask> userTasks = NHibernateHelper.userTaskManager.GetListByUid(Uid);
            if (tasks.Count != userTasks.Count)
            {
                userTasks.Clear();
                for (int i = 0; i < tasks.Count; i++)
                {
                    Task task = tasks[i];
                    UserTask userTask = new UserTask()
                    {
                        isover = 0,
                        progress = 0,
                        Uid = Uid,
                        task_id = task.task_id,
                    };
                    userTasks.Add(userTask);
                    NHibernateHelper.userTaskManager.Add(userTask);
                }
            }

            List<UserTaskJsonObject> userTaskJsonObjects = new List<UserTaskJsonObject>();
            UserTaskJsonObject userTaskJsonObject;
            for (int i = 0; i < userTasks.Count; i++)
            {
                UserTask userTask = userTasks[i];
                Task task = tasks[i];
                userTaskJsonObject = new UserTaskJsonObject();
                userTaskJsonObject.content = task.content;
                userTaskJsonObject.title = task.title;
                userTaskJsonObject.reward = task.reward;
                userTaskJsonObject.target = task.target;

                userTaskJsonObject.isover = userTask.isover;
                userTaskJsonObject.progress = userTask.progress;
                userTaskJsonObject.task_id = userTask.task_id;
                userTaskJsonObjects.Add(userTaskJsonObject);
            }
            OperatorSuccess(userTaskJsonObjects, responseData);
        }


        //数据库操作成功
        private void OperatorSuccess(List<UserTaskJsonObject> userTaskJsonObjects, JObject responseData)
        {

            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("task_list", JsonConvert.SerializeObject(userTaskJsonObjects));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}