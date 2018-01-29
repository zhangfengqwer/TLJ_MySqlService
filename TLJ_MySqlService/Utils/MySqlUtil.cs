using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using TLJCommon;
using TLJ_MySqlService.Handler;
using Task = NhInterMySQL.Model.Task;

namespace TLJ_MySqlService.Utils
{
    public class MySqlUtil
    {
        public static void UpdateUserTask(string uid)
        {
            List<Task> tasks = NHibernateHelper.taskManager.GetAll().ToList();
            List<UserTask> userTasks = NHibernateHelper.userTaskManager.GetListByUid(uid);
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
                        Uid = uid,
                        task_id = task.task_id,
                    };
                    userTasks.Add(userTask);
                    NHibernateHelper.userTaskManager.Add(userTask);
                }
            }
        }

        public static void ConfigExpenseGold(string uid, int goldExpense)
        {
            var config = NHibernateHelper.commonConfigManager.GetByUid(uid);
            if (config == null)
            {
                config = ModelFactory.CreateConfig(uid);
            }
            config.expense_gold_daily += goldExpense;
            NHibernateHelper.commonConfigManager.Update(config);

            UserTask userTask = NHibernateHelper.userTaskManager.GetUserTask(uid, 216);
            if (userTask == null)
            {
                var user = NHibernateHelper.userManager.GetByUid(uid);
                if (user == null)
                {
                    MySqlService.log.Error($"改用户未注册{uid}");
                }
                else
                {
                    if (user.IsRobot == 1)
                    {
                        MySqlService.log.Info($"机器人没有任务{uid}");
                    }
                    else
                    {
                        MySqlService.log.Error($"没有改用户任务{uid}，216");
                    }
                }
                
              
            }
            else
            {
                if (userTask.isover == 0)
                {
                    NhInterMySQL.Model.Task task = NHibernateHelper.taskManager.GetTask(216);
                    if (task.target - userTask.progress >= 1)
                    {
                        //任务进度加
                        userTask.progress += goldExpense;
                        if (userTask.progress > 500) userTask.progress = 500;
                        if (NHibernateHelper.userTaskManager.Update(userTask))
                        {
                            MySqlService.log.Info("更新每日花费任务成功");
                        }
                        else
                        {
                            MySqlService.log.Info("更新任务失败:" + uid);
                        }
                    }
                    else
                    {
                        MySqlService.log.Info("玩家任务已完成，请领取奖励:" + uid + " ");
                    }
                }
                else
                {
                    MySqlService.log.Info("任务奖励已领取:" + uid);
                }
            }

        }


        /// <summary>
        /// 添加道具  prop为如下格式："1:123;2:123;106:2"
        /// </summary>
        /// <param name="prop">以如下格式："1:123;2:123"</param>
        /// <param name="uid"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool AddProp(string uid, string prop,string reason)
        {
            List<string> list = new List<String>();
            CommonUtil.splitStr(prop, list, ';');
            for (int i = 0; i < list.Count; i++)
            {
                string[] strings = list[i].Split(':');
                int propId = Convert.ToInt32(strings[0]);
                int propNum = Convert.ToInt32(strings[1]);
                if (!ChangeProp(uid, propId, propNum, reason)) return false;
            }
            return true;
        }

        /// <summary>
        /// 1.是金币,2.是话费，4徽章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="propId">1,2...</param>
        /// <param name="propNum"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool ChangeProp(string uid, int propId, int propNum,string reason)
        {
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
            if (userInfo == null)
            {
                MySqlService.log.Warn("用户不存在");
                return false;
            }

            if (propId == 1)
            {
                userInfo.Gold += propNum;
                if (userInfo.Gold <= 0)
                {
                    User user = NHibernateHelper.userManager.GetByUid(uid);
                    if (user.IsRobot == 1)
                    {
                        userInfo.Gold = new Random().Next(80000,100000);
                    }
                    else
                    {
                        userInfo.Gold = 0;
                    }

                }
                
                if (userInfo.Gold >= 0)
                {
                    if (!NHibernateHelper.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn("添加金币失败");
                        return false;
                    }
                    else
                    {
                        int afterGold = userInfo.Gold;
                        int changeGold = propNum;

                        StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.GOLD, reason,afterGold - changeGold, changeGold, afterGold);

//                        string msg = $"改变了{propNum}金币";
//                        LogUtil.Log(uid, MyCommon.OpType.CHANGE_WEALTH, msg);
                    }
                    //发送奖励金;
                    SendSupplyGold(uid);
                }
                else
                {
                    MySqlService.log.Warn("金币不足");
                    return false;
                }
            }
            else if (propId == 2)
            {
                userInfo.YuanBao += propNum;
               
                if (userInfo.YuanBao >= 0)
                {
                    if (!NHibernateHelper.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn("添加元宝失败");
                        return false;
                    }
                    else
                    {
                        //记录玩家财富变化日志
                        int after = userInfo.YuanBao;
                        int change = propNum;

                        StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.YUANBAO, reason, after - change, change, after);
                        //                        string msg = $"改变了{propNum}元宝";
                        //                        LogUtil.Log(uid, MyCommon.OpType.CHANGE_WEALTH, msg);
                    }
                }
                else
                {
                    MySqlService.log.Warn("元宝不足");
                    return false;
                }
                  
            }else if (propId == 110)
            {
                userInfo.Medel += propNum;
                if (userInfo.Medel >= 0)
                {
                    if (!NHibernateHelper.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn($"添加徽章失败-medal:{userInfo.Medel}-改变的数量:{propNum}");
                        return false;
                    }
                    else
                    {
                        //记录玩家财富变化日志
                        int after = userInfo.Medel;
                        int change = propNum;

                        StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.Medal, reason, after - change, change, after);
                        //                        string msg = $"改变了{propNum}徽章";
                        //                        LogUtil.Log(uid, MyCommon.OpType.CHANGE_WEALTH, msg);
                    }
                }
                else
                {
                    MySqlService.log.Warn($"徽章不足-medal:{userInfo.Medel}-改变的数量:{propNum}");
                    return false;
                }
            }
            else
            {
                UserProp userProp = NHibernateHelper.userPropManager.GetUserProp(uid, propId);
                if (userProp == null)
                {
                    userProp = new UserProp()
                    {
                        Uid = uid,
                        PropId = propId,
                        PropNum = propNum
                    };
                    if (!NHibernateHelper.userPropManager.Add(userProp))
                    {
                        MySqlService.log.Warn($"添加道具失败-propId：{propId}-propNum：{propNum}");
                        return false;
                    }
                    else
                    {
//                        string msg = $"改变了{propId}道具,{propNum}个";
//                        LogUtil.Log(uid, MyCommon.OpType.CHANGE_WEALTH, msg);
                    }
                }
                else
                {
                    userProp.PropNum += propNum;
                    if (userProp.PropNum >= 0)
                    {
                        if (!NHibernateHelper.userPropManager.Update(userProp))
                        {
                            MySqlService.log.Warn("更新道具失败");
                            return false;
                        }
                        else
                        {
//                            string msg = $"改变了{propId}道具,{propNum}个";
//                            LogUtil.Log(uid, MyCommon.OpType.CHANGE_WEALTH, msg);
                        }
                    }
                    else
                    {
                        MySqlService.log.Warn("道具不足");
                        return false;
                    }
                }
            }
            return true;
        }

        //tag(string)：SupplyGold
        //uid(string)
        //todayCount(int)
        //goldNum(int)
        public static void SendSupplyGold(string uid)
        {
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
            if (userInfo.Gold < 1500)
            {
                var config = NHibernateHelper.commonConfigManager.GetByUid(uid);
                if (config == null) config = ModelFactory.CreateConfig(uid);
                if (config.free_gold_count > 0)
                {
                    userInfo.Gold += 2000;
                    config.free_gold_count--;
                    NHibernateHelper.commonConfigManager.Update(config);
                    NHibernateHelper.userInfoManager.Update(userInfo);

                    MySqlService.log.Info($"{uid}发放补助金");
                    //给logic服务器推送
                    IntPtr connId;
                    if (MySqlService.serviceDic.TryGetValue(TljServiceType.LogicService, out connId))
                    {
                        var jObject = new JObject();
                        jObject.Add(MyCommon.TAG, Consts.Tag_SupplyGold);
                        jObject.Add(MyCommon.UID, uid);
                        int temp = 0;
                        if (config.free_gold_count == 2)
                        {
                            temp = 1;
                        }
                        else if (config.free_gold_count == 1)
                        {
                            temp = 2;
                        }
                        else if (config.free_gold_count == 0)
                        {
                            temp = 3;
                        }
                        jObject.Add("todayCount", temp);
                        jObject.Add("goldNum", 2000);
                        MySqlService.Instance().sendMessage(connId, jObject.ToString());
                        MySqlService.log.Info($"主动发送给logic：{jObject.ToString()}");


                        //记录玩家财富变化日志
                        int afterGold = userInfo.Gold;
                        int changeGold = 2000;

                        StatictisLogUtil.ChangeWealth(userInfo.Uid, userInfo.NickName, MyCommon.GOLD, "奖励金发放", afterGold - changeGold, changeGold, afterGold);
                    }
                }
            }
        }
    }
}