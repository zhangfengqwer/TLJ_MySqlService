using HPSocketCS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using NhInterMySQL;
using NhInterSqlServer.Model;
using TLJCommon;
using TLJ_MySqlService.Handler;
using TLJ_MySqlService.Utils;

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
        public static TcpPackServer m_tcpServer = new TcpPackServer();
//        TcpServer m_tcpServer = new TcpServer();

        public static string TAG = "MySqlService";
        public static Logger log = LogManager.GetLogger("MySqlService");
        public static Logger DBLog = LogManager.GetLogger("DBLog");
        public static Dictionary<string, BaseHandler> handlerDic = new Dictionary<string, BaseHandler>();
        public static Dictionary<TljServiceType, IntPtr> serviceDic = new Dictionary<TljServiceType, IntPtr>();

        public static List<PVPGameRoom> PvpGameRooms;
        public static List<Goods> ShopData;
        public static List<SignConfig> SignConfigs;
        public static List<TurnTable> TurnTables;
        public static List<TurnTable> FreeTurnTables;
        public static List<TurnTable> MedalTurnTables;

        public static List<VipData> VipDatas;
        public static List<MedalExchargeRewardData> medalExchargeRewardDatas;
        public static string AdminAccount = "admin";
        public static string AdminPassWord = "jinyou123";
        public static bool IsTest = true;


        public MySqlService()
        {
            InitializeComponent();
        }

        private static MySqlService instance;

        public static MySqlService Instance()
        {
            if (instance == null)
            {
                instance = new MySqlService();
            }
            return instance;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                InitLog();
                InitHandler();
                InitCommomData();
                log.Info($"初始数据完成:PvpGameRooms:{PvpGameRooms.Count},ShopData:{ShopData.Count},SignConfigs:{SignConfigs.Count},TurnTables:{TurnTables.Count}");
                NetConfig.init();
                InitData();
                InitService();
                StartService();
            }
            catch (Exception e)
            {
                log.Error("OnStart:" + e);
            }
        }

        /// <summary>
        /// 初始化 webapi相关的参数
        /// </summary>
        private void InitData()
        {
            if ("10.224.4.135".Equals(NetConfig.s_mySqlService_ip))
            {
                IsTest = true;
                log.Info("测试环境");
                HttpUtil.sendKey = "sy";
                HttpUtil.phoneFeeKey = "sy";
                HttpUtil.clientip = "58.210.102.138";
            }
            else
            {
                IsTest = false;
                log.Info("正式环境");
                HttpUtil.sendKey = "sy123";
                HttpUtil.phoneFeeKey = "fw123";
                HttpUtil.clientip = "139.196.193.185";
            }
        }
            
        private void InitCommomData()
        {
            PvpGameRooms = NHibernateHelper.PVPGameRoomManager.GetAll().ToList();
            ShopData = NHibernateHelper.goodsManager.GetAll().ToList().ToList();
            SignConfigs = NHibernateHelper.signConfigManager.GetAll().ToList();
            TurnTables = NHibernateHelper.turnTableManager.GetAll().ToList();

            MedalTurnTables = new List<TurnTable>();
            FreeTurnTables = new List<TurnTable>();
            foreach (var turnTable in TurnTables)
            {
                if (turnTable.id > 50)
                {
                    MedalTurnTables.Add(turnTable);
                }
                else
                {
                    FreeTurnTables.Add(turnTable);
                }
            }


            InitVipRewardData();
            InitMedalDuiHuanRewardData();

            Sign30Data.getInstance().init();
        }

        private static void InitMedalDuiHuanRewardData()
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "MedalDuiHuanReward.json");
            string str = sr.ReadToEnd();
            sr.Close();
            medalExchargeRewardDatas = JsonConvert.DeserializeObject<List<MedalExchargeRewardData>>(str);
        }

        private static void InitVipRewardData()
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "VipRewardData.json");
            string str = sr.ReadToEnd();
            sr.Close();
            VipDatas = JsonConvert.DeserializeObject<List<VipData>>(str);
        }

        /// <summary>
        /// InitHandler
        /// </summary>
        private void InitHandler()
        {
            Assembly assembly = typeof(MySqlService).Assembly;

            Type[] types = assembly.GetTypes();

            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(HandlerAttribute), false);

                foreach (var attr in attributes)
                {
                    HandlerAttribute handlerAttribute = (HandlerAttribute) attr;
                    object obj = Activator.CreateInstance(type);

                    BaseHandler baseHandler = obj as BaseHandler;
                    if (baseHandler == null)
                    {
                        log.Warn($"没有继承BaseHandler:{type.Name}");
                        continue;
                    }

                    if (!handlerDic.ContainsKey(handlerAttribute.Tag))
                    {
                        handlerDic.Add(handlerAttribute.Tag, baseHandler);
                    }
                    else
                    {
                        log.Warn($"key值重复:{handlerAttribute.Tag}");
                    }
                }
            }
            log.Info($"handlerCount:{handlerDic.Count}");
        }

        public void InitLog()
        {
            log.Info("log启动");
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
            m_tcpServer.MaxPackSize = Consts.MaxPackSize;
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
                    log.Info("TCP服务启动成功,当前服务器ip," + NetConfig.s_mySqlService_ip + ":" + NetConfig.s_mySqlService_port);
                }
                else
                {
                    log.Warn("TCP服务启动失败ip," + NetConfig.s_mySqlService_ip + ":" + NetConfig.s_mySqlService_port);
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
                log.Info("有客户端连接--ip = " + ip + "--port = " + port + "\nconnid:" + connId + ",pClient:" + pClient);
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
            log.Info($"收到消息，connId:{connId}");
            try
            {
                ReceiveObj obj = new ReceiveObj(connId, bytes);
                new System.Threading.Tasks.Task(() => { doAskCilentReq(obj); }).Start();
//                new Thread(() => { doAskCilentReq(obj); }).Start();
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

            TljServiceType type = TljServiceType.None;
            foreach (var key in serviceDic.Keys)
            {
                IntPtr value;
                if (serviceDic.TryGetValue(key, out value))
                {
                    if (connId == value)
                    {
                        type = key;
                    }
                }
            }

            if (type != TljServiceType.None)
            {
                serviceDic.Remove(type);
            }

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

            if (tag.ToString().Equals(Consts.Tag_Login))
            {
                if (!serviceDic.ContainsKey(TljServiceType.LoginService))
                {
                    serviceDic.Add(TljServiceType.LoginService, receiveObj.m_connId);
                }
            }
            else if (tag.ToString().Equals(Consts.Tag_GetAIList))
            {
                if (!serviceDic.ContainsKey(TljServiceType.LoginService))
                {
                    serviceDic.Add(TljServiceType.PlayService, receiveObj.m_connId);
                }
            }
            else if (tag.ToString().Equals(Consts.Tag_GetTurntable))
            {
                if (!serviceDic.ContainsKey(TljServiceType.LoginService))
                {
                    serviceDic.Add(TljServiceType.LogicService, receiveObj.m_connId);
                }
            }

            BaseHandler baseHandler;
            if (handlerDic.TryGetValue(tag.ToString(), out baseHandler))
            {
                try
                {
                    var onRequest = baseHandler.OnResponse(text);
                    if (onRequest != null)
                    {
                        sendMessage(receiveObj.m_connId, onRequest);
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
                log.Info("发送消息：" + text + "\nconnid:" + connId);
            }
            else
            {
                log.Error("发送失败,数据长度:" + bytes.Length+",data:"+text);
            }
        }
    }
}