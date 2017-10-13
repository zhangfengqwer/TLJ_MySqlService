using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using TLJ_MySqlService;
using TLJ_MySqlService.Model;
using Zfstu;
using Zfstu.Manager;
using Zfstu.Model;

namespace UpdateSignMonday
{
    class Program
    {
        private static MySqlManager<Sign> signManager = new MySqlManager<Sign>();
        private static MySqlManager<User> userManager = new MySqlManager<User>();
        private static MySqlManager<UserInfo> userInfoManager = new MySqlManager<UserInfo>();
        private static MySqlManager<UserEmail> userEmailManager = new MySqlManager<UserEmail>();
        private static MySqlManager<UserGame> userGameManager = new MySqlManager<UserGame>();
        private static MySqlManager<UserProp> userPropManager = new MySqlManager<UserProp>();
        private static MySqlManager<Notice> noticeManager = new MySqlManager<Notice>();
        private static MySqlManager<UserNotice> userNoticeManager = new MySqlManager<UserNotice>();
        
        static void Main(string[] args)
        {
//            UserProp userProp = userPropManager.GetUserProp("6506476654", 109);
//            if (userProp == null || userProp.PropNum <= 0)
//            {
//                Console.WriteLine("shibai");
//            }
//            else
//            {
//                Console.WriteLine("yuo");
//            }
            //            Console.WriteLine(DateTime.Now.ToLongDateString()+" "+ DateTime.Now.ToLongTimeString());
                //            Console.ReadKey();
                List<UserProp> userProps = userPropManager.GetListByUid("6506476654");
            ICollection<Notice> collection = noticeManager.GetAll();
            List<UserNotice> userNotices = userNoticeManager.GetListByUid("6506476654");
            Console.WriteLine(collection.Count+" "+ userNotices.Count);
//            //添加道具
//            UserProp userProp1 = userPropManager.GetUserProp("6506476654",105);
//            if (userProp1 == null)
//            {
//
//            }
//            else
//            {
//                
//            }
//
//
//            UserProp userProp = new UserProp()
//            {
//                Uid = "6506476654",
//                PropId = 109,
//                PropNum = 2,
//                Type = 0
//            };
//
//            userPropManager.Add(userProp);

            //            foreach (var prop in userProps)
            //            {
            //                Console.WriteLine(prop.PropId);
            //            }
            Console.ReadKey();
            //            new Thread(UpdateSignDays).Start();
        }

        private static void UpdateSignDays()
        {
            ICollection<Sign> signs = signManager.GetAll();
            foreach (var sign in signs)
            {
                sign.SignWeekDays = 0;
                sign.UpdateTime = DateTime.Now;
                signManager.Update(sign);
            }
        }


    }
}