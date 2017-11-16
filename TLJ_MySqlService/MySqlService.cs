using HPSocketCS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TLJ_MySqlService.Handler;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService
{
    class ReceiveObj
    {
        public IntPtr m_connId;
        public byte[] m_bytes;

        public ReceiveObj(IntPtr connId, byte[] bytes)
        {
            m_connId = connId;
            m_bytes = bytes;
        }
    }

    public partial class MySqlService : ServiceBase
    {
        TcpPackServer m_tcpServer = new TcpPackServer();
//        TcpServer m_tcpServer = new TcpServer();

        public static string TAG = "MySqlService";
        public static Logger log = LogManager.GetLogger("MySqlService");
        public static Logger DBLog = LogManager.GetLogger("DBLog");
        public static Dictionary<string, BaseHandler> handlerDic = new Dictionary<string, BaseHandler>();

        public static MySqlManager<UserTask> userTaskManager = new MySqlManager<UserTask>();
        public static MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();
        public static MySqlManager<UserGame> userGameManager = new MySqlManager<UserGame>();
        public static MySqlManager<User> userManager = new MySqlManager<User>();
        public static MySqlManager<UserProp> userPropManager = new MySqlManager<UserProp>();
        public static MySqlManager<UserRealName> userRealNameManager = new MySqlManager<UserRealName>();
        public static MySqlManager<UserNotice> userNoticeManager = new MySqlManager<UserNotice>();
        public static MySqlManager<UserEmail> userEmailManager = new MySqlManager<UserEmail>();
        public static MySqlManager<Goods> goodsManager = new MySqlManager<Goods>();
        public static MySqlManager<Notice> noticeManager = new MySqlManager<Notice>();
        public static MySqlManager<Sign> signManager = new MySqlManager<Sign>();
        public static MySqlManager<PVPGameRoom> PVPGameRoomManager = new MySqlManager<PVPGameRoom>();
        public static MySqlManager<Task> taskManager = new MySqlManager<Task>();
        public static MySqlManager<SignConfig> signConfigManager = new MySqlManager<SignConfig>();
        public static MySqlManager<Prop> propManager = new MySqlManager<Prop>();
        public static MySqlManager<MyLog> logManager = new MySqlManager<MyLog>();
        public static MySqlManager<CommonConfig> commonConfigManager = new MySqlManager<CommonConfig>();
        public static MySqlManager<TurnTable> turnTableManager = new MySqlManager<TurnTable>();

        public static List<PVPGameRoom> PvpGameRooms;
        public static List<Goods> ShopData;
        public static List<SignConfig> SignConfigs;
        public static List<TurnTable> TurnTables;

        public static string AdminAccount = "admin";
        public static string AdminPassWord = "jinyou123";


        public MySqlService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                InitLog();
                InitHandler();
                log.Info("Handler初始化完成");
                InitCommomData();
                log.Info($"初始数据完成:PvpGameRooms:{PvpGameRooms.Count},ShopData:{ShopData.Count},SignConfigs:{SignConfigs.Count},TurnTables:{TurnTables.Count}");
                NetConfig.init();
                InitService();
                StartService();
               

            }
            catch (Exception e)
            {
                log.Error("OnStart:" + e);
            }
        }

        private void InitCommomData()
        {
            PvpGameRooms = PVPGameRoomManager.GetAll().ToList();
            ShopData = goodsManager.GetAll().ToList();
            SignConfigs = signConfigManager.GetAll().ToList();
            TurnTables = turnTableManager.GetAll().ToList();
        }

        public static void AddHandler(BaseHandler handler)
        {
            handlerDic.Add(handler.Tag, handler);
        }

        /// <summary>
        /// 初始化Handler
        /// </summary>
        private void InitHandler()
        {
            LoginHandler loginHandler = new LoginHandler();
            handlerDic.Add(loginHandler.Tag, loginHandler);

            ThirdLoginHandler thirdLoginHandler = new ThirdLoginHandler();
            handlerDic.Add(thirdLoginHandler.Tag, thirdLoginHandler);

            RegisterHandler registerHandler = new RegisterHandler();
            handlerDic.Add(registerHandler.Tag, registerHandler);

            GetSignRecordHandler getSignRecordHandler = new GetSignRecordHandler();
            handlerDic.Add(getSignRecordHandler.Tag, getSignRecordHandler);

            SignHandler signHandler = new SignHandler();
            handlerDic.Add(signHandler.Tag, signHandler);

            GetUserInfoHandler getUserInfoHandler = new GetUserInfoHandler();
            handlerDic.Add(getUserInfoHandler.Tag, getUserInfoHandler);

            GetEmailHandler getEmailHandler = new GetEmailHandler();
            handlerDic.Add(getEmailHandler.Tag, getEmailHandler);

            AddEmailHandler addEmailHandler = new AddEmailHandler();
            handlerDic.Add(addEmailHandler.Tag, addEmailHandler);

            ReadEmailHandler readEmailHandler = new ReadEmailHandler();
            handlerDic.Add(readEmailHandler.Tag, readEmailHandler);

            DeleteEmailHandler deleteEmailHandler = new DeleteEmailHandler();
            handlerDic.Add(deleteEmailHandler.Tag, deleteEmailHandler);

            OneKeyReadEmailHandler OneKeyReadEmailHandler = new OneKeyReadEmailHandler();
            handlerDic.Add(OneKeyReadEmailHandler.Tag, OneKeyReadEmailHandler);

            OneKeyDeleteEmailHandler OneKeyDeleteEmailHandler = new OneKeyDeleteEmailHandler();
            handlerDic.Add(OneKeyDeleteEmailHandler.Tag, OneKeyDeleteEmailHandler);

            GetUserBagHandler getUserBagHandler = new GetUserBagHandler();
            handlerDic.Add(getUserBagHandler.Tag, getUserBagHandler);

            //使用道具
            UsePropHandler usePropHandler = new UsePropHandler();
            handlerDic.Add(usePropHandler.Tag, usePropHandler);

            //使用话费
            UseHuaFeiHandler useHuaFeiHandler = new UseHuaFeiHandler();
            handlerDic.Add(useHuaFeiHandler.Tag, useHuaFeiHandler);

            GetUseNoticeHandler getUseNoticeHandler = new GetUseNoticeHandler();
            handlerDic.Add(getUseNoticeHandler.Tag, getUseNoticeHandler);

            ReadNoticeHandler readNoticeHandler = new ReadNoticeHandler();
            handlerDic.Add(readNoticeHandler.Tag, readNoticeHandler);

            GetGoodsHandler getGoodsHandler = new GetGoodsHandler();
            handlerDic.Add(getGoodsHandler.Tag, getGoodsHandler);

            BuyGoodsHandler buyGoodsHandler = new BuyGoodsHandler();
            handlerDic.Add(buyGoodsHandler.Tag, buyGoodsHandler);

            GetTaskHandler getTaskHandler = new GetTaskHandler();
            handlerDic.Add(getTaskHandler.Tag, getTaskHandler);

            CompleteTaskHandler completeTaskHandler = new CompleteTaskHandler();
            handlerDic.Add(completeTaskHandler.Tag, completeTaskHandler);

            ProgressTaskHandler progressTaskHandler = new ProgressTaskHandler();
            handlerDic.Add(progressTaskHandler.Tag, progressTaskHandler);

            GetOtherUserInfoHandler getOtherUserInfoHandler = new GetOtherUserInfoHandler();
            handlerDic.Add(getOtherUserInfoHandler.Tag, getOtherUserInfoHandler);

            RealNameHandler realNameHandler = new RealNameHandler();
            handlerDic.Add(realNameHandler.Tag, realNameHandler);

            UseLaBaHandler useLaBaHandler = new UseLaBaHandler();
            handlerDic.Add(useLaBaHandler.Tag, useLaBaHandler);

            SendSmsHandler sendSmsHandler = new SendSmsHandler();
            handlerDic.Add(sendSmsHandler.Tag, sendSmsHandler);

            CheckSmsHandler checkSmsHandler = new CheckSmsHandler();
            handlerDic.Add(checkSmsHandler.Tag, checkSmsHandler);

            GetPVPGameDataHandler getPVPGameDataHandler = new GetPVPGameDataHandler();
            handlerDic.Add(getPVPGameDataHandler.Tag, getPVPGameDataHandler);

            GetRankHandler getRankHandler = new GetRankHandler();
            handlerDic.Add(getRankHandler.Tag, getRankHandler);

            ChangeUserWealth changeUserWealth = new ChangeUserWealth();
            handlerDic.Add(changeUserWealth.Tag, changeUserWealth);

            GetRobotHandler getRobotHandler = new GetRobotHandler();
            handlerDic.Add(getRobotHandler.Tag, getRobotHandler);

            RecordUserGameDataHandler recordUserGameDataHandler = new RecordUserGameDataHandler();
            handlerDic.Add(recordUserGameDataHandler.Tag, recordUserGameDataHandler);

            UseBuffPropHandler useBuffPropHandler = new UseBuffPropHandler();
            handlerDic.Add(useBuffPropHandler.Tag, useBuffPropHandler);

            GetWXUserInfoHandler getWXUserInfoHandler = new GetWXUserInfoHandler();
            handlerDic.Add(getWXUserInfoHandler.Tag, getWXUserInfoHandler);

            BuyYuanBaoHandler buyYuanBaoHandler = new BuyYuanBaoHandler();
            handlerDic.Add(buyYuanBaoHandler.Tag, buyYuanBaoHandler);

            SetUserSecondPSWHandler setUserSecondPSWHandler = new SetUserSecondPSWHandler();
            handlerDic.Add(setUserSecondPSWHandler.Tag, setUserSecondPSWHandler);

            GetTurnTableDataHandler getTurnTableDataHandler = new GetTurnTableDataHandler();
            handlerDic.Add(getTurnTableDataHandler.Tag, getTurnTableDataHandler);

            UseTurnTableHandler useTurnTableHandler = new UseTurnTableHandler();
            handlerDic.Add(useTurnTableHandler.Tag, useTurnTableHandler);
        }

        public void InitLog()
        {
            log.Info("1");
        }

        private void InitService()
        {
            // 设置服务器事件
            m_tcpServer.OnPrepareListen += new TcpServerEvent.OnPrepareListenEventHandler(OnPrepareListen);
            m_tcpServer.OnAccept += new TcpServerEvent.OnAcceptEventHandler(OnAccept);
            m_tcpServer.OnSend += new TcpServerEvent.OnSendEventHandler(OnSend);
            // 两个数据到达事件的一种
            //server.OnPointerDataReceive += new TcpServerEvent.OnPointerDataReceiveEventHandler(OnPointerDataReceive);
            // 两个数据到达事件的一种
            m_tcpServer.OnReceive += new TcpServerEvent.OnReceiveEventHandler(OnReceive);

            m_tcpServer.OnClose += new TcpServerEvent.OnCloseEventHandler(OnClose);
            m_tcpServer.OnShutdown += new TcpServerEvent.OnShutdownEventHandler(OnShutdown);
            //设置包头标识,与对端设置保证一致性
            m_tcpServer.PackHeaderFlag = 0xff;
            // 设置最大封包大小
            m_tcpServer.MaxPackSize = 0x2000;
        }

        private void StartService()
        {
            try
            {
                m_tcpServer.IpAddress = NetConfig.s_mySqlService_ip;
                m_tcpServer.Port = (ushort) NetConfig.s_mySqlService_port;
//                m_tcpServer.IpAddress = "10.224.5.68";
//                m_tcpServer.Port = 60005;

                // 启动服务
                if (m_tcpServer.Start())
                {
                    log.Info("TCP服务启动成功,当前服务器i-:"+ NetConfig.s_mySqlService_ip+":"+ NetConfig.s_mySqlService_port);
                }
                else
                {
                    log.Warn("TCP服务启动失败");
                }
            }
            catch (Exception ex)
            {
                log.Error("TCP服务启动异常:" + ex.Message);
            }
        }

        protected override void OnStop()
        {
            log.Info("关闭服务");
        }

        HandleResult OnPrepareListen(IntPtr soListen)
        {
            return HandleResult.Ok;
        }


        // 客户进入了
        HandleResult OnAccept(IntPtr connId, IntPtr pClient)
        {
            // 获取客户端ip和端口
            string ip = string.Empty;
            ushort port = 0;
            if (m_tcpServer.GetRemoteAddress(connId, ref ip, ref port))
            {
                log.Info("有客户端连接--ip = " + ip + "--port = " + port);
            }
            else
            {
                log.Warn("获取客户端ip地址出错");
            }

            return HandleResult.Ok;
        }


        HandleResult OnPointerDataReceive(IntPtr connId, IntPtr pData, int length)
        {
            // 数据到达了
            try
            {
                if (m_tcpServer.Send(connId, pData, length))
                {
                    return HandleResult.Ok;
                }

                return HandleResult.Error;
            }
            catch (Exception)
            {
                return HandleResult.Ignore;
            }
        }

        HandleResult OnReceive(IntPtr connId, byte[] bytes)
        {
            try
            {
                ReceiveObj obj = new ReceiveObj(connId, bytes);
                System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {
                    doAskCilentReq(obj);
                });
                t.Start();
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            return HandleResult.Ok;
        }

        HandleResult OnSend(IntPtr connId, byte[] bytes)
        {
            return HandleResult.Ok;
        }


        HandleResult OnClose(IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            log.Info("与客户端断开:" + connId);
            return HandleResult.Ok;
        }

        // 服务关闭
        HandleResult OnShutdown()
        {
            //调用关闭的地方已经记录日志了，这里不需要重复记录
            //addDebugLog.WriteLine("OnShutdown");
//           log.addDebugLog("TCP服务关闭成功");

            return HandleResult.Ok;
        }

        // 处理客户端的请求
        void doAskCilentReq(object obj)
        {
            // 模拟耗时操作，比如数据库操作，IO操作
            // Thread.Sleep(3000);

            ReceiveObj receiveObj = (ReceiveObj) obj;
            string text = Encoding.UTF8.GetString(receiveObj.m_bytes, 0, receiveObj.m_bytes.Length);

            log.Info("收到消息：" + text);

            JObject jo;
            try
            {
                jo = JObject.Parse(text);
            }
            catch (JsonReaderException ex)
            {
                // 传过来的数据不是json格式的，一律不理
                log.Warn("客户端传来非json数据：" + text);
                return;
            }
            JToken tag;
            bool isSuccess = jo.TryGetValue("tag", out tag);
            if (!isSuccess)
            {
                log.Warn("传入的值没有TAG");
                return;
            }
            BaseHandler baseHandler;
            handlerDic.TryGetValue(tag.ToString(), out baseHandler);
            if (baseHandler != null)
            {
                try
                {
                    var onRequest = baseHandler.OnResponse(text);
                    if (onRequest != null)
                    {
                        sendMessage(receiveObj.m_connId, onRequest);
                    }
                    else
                    {
                    }
                }
                catch (Exception e)
                {
                    log.Warn("处理回调数据错误：" + e);
                }
            }
            else
            {
                log.Warn("没有得到tag对应的handler:" + tag);
            }
        }

        // 发送消息
        public void sendMessage(IntPtr connId, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            if (m_tcpServer.Send(connId, bytes, bytes.Length))
            {
                //OnSend函数会记录日志，这里不需要重复记录了
                //addDebugLog.WriteLine("发送消息：{0}，{1}", connId, text);
                log.Info("发送消息：" + text);
            }
            else
            {
                log.Error("发送失败:"+ bytes.Length);
            }
        }
    }
}