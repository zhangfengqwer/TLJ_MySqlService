using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJCommon;
using NhInterMySQL.Model;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_QuickRegister)]
    class RegisterHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            LoginReq login = null;
            try
            {
                login = JsonConvert.DeserializeObject<LoginReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string _tag = login.tag;
            int _connId = login.connId;
            string _username = login.account;
            string _userpassword = login.password;
            string channelname = login.channelname;
            string machineId = login.mac;
            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_userpassword) ||
                string.IsNullOrWhiteSpace(channelname) || string.IsNullOrWhiteSpace(machineId))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData;
            _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);
            User _user = new User() {Username = _username, Userpassword = _userpassword};
            RegisterSQL(_user, channelname, machineId, _responseData);
            return _responseData.ToString();
        }

        //注册 数据库操作
        private void RegisterSQL(User user, string channelname, string machineId, JObject responseData)
        {
            User userByName = NHibernateHelper.userManager.GetByName(user.Username);

            List<User> users = MySqlManager<User>.Instance.GetByPorperty("MachineId", machineId);
            if (users?.Count >= 10)
            {
                OperatorFail(responseData, "每台手机最多注册十个账号");
                return;
            }

            if (userByName != null)
            {
                OperatorFail(responseData, "用户已存在");
            }
            else
            {
                string uid = UidUtil.createUID();
                user.Uid = uid;
                user.ChannelName = channelname;
                user.ThirdId = "";
                user.Secondpassword = "";
                user.IsRobot = 0;
                user.Userpassword = CommonUtil.CheckPsw(user.Userpassword);
                user.CreateTime = DateTime.Now;
                user.MachineId = machineId;

                var userEmail = new UserEmail()
                {
                    Uid = uid,
                    Title = "新用户奖励",
                    Content = "欢迎来到疯狂升级，为您送上1000金币，快去对战吧!",
                    Reward = "1:1000",
                    State = 0,
                    CreateTime = DateTime.Now,
                };


                //注册用户数据 并 注册新手邮箱
                if (NHibernateHelper.userManager.Add(user) && NHibernateHelper.userEmailManager.Add(userEmail))
                {
                    OperatorSuccess(user, responseData);
                }
                else
                {
                    OperatorFail(responseData, "用户注册失败");
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(User user, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add(MyCommon.UID, user.Uid);

            MySqlUtil.UpdateUserTask(user.Uid);
            ProgressTaskHandler.ProgressTaskSql(208, user.Uid);

            StatisticsHelper.StatisticsRegister(user.Uid);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}