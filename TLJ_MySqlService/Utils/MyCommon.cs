public class MyCommon
{
    public enum Platform
    {
        Platform_Official,
    }

    public enum Code
    {
        Code_OK = 1000,
        Code_LoginFail,
    }

    public class OpType 
    {
        public static readonly string BUYGOLD = "buy_gold";
        public static readonly string BUYYUANBAO = "buy_yuanbao";
        public static readonly string BUYPROP = "buy_prop";
        public static readonly string RECHARGE_PHONEFEE = "recharge_phonefee";
        public static readonly string CHANGE_WEALTH = "change_wealth";
    }

    public static string Tag_Login = "Login";
    public static string Tag_QuickRegister = "QuickRegister";
    public static string Tag_Sign = "Sign";
    public static readonly string ACCOUNT = "account";
    public static readonly string PASSWORD = "password";
    public static readonly string UID = "uid";
    public static readonly string TAG = "tag";
    public static readonly string CODE = "code";
    public static readonly string CONNID = "connId";
    public static readonly string SIGNWEEKDAYS = "signWeekDays";
    public static readonly string UPDATETIME = "updateTime";
    public static readonly string NAME = "name";
    public static readonly string PHONE = "phone";
    public static readonly string GOLD = "gold";
    public static readonly string YUANBAO = "yuanbao";
    public static readonly string HEAD = "head";
    public static readonly string GAMEDATA = "gameData";
    public static readonly string PLATFORM = "platform";
    public static readonly string EMAIL_ID = "email_id";
}