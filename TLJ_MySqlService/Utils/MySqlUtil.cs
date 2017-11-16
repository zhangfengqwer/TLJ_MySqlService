using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zfstu.Model;

namespace TLJ_MySqlService.Utils
{
    public class MySqlUtil
    {
        /// <summary>
        /// 添加道具  prop为如下格式："1:123;2:123;106:2"
        /// </summary>
        /// <param name="prop">以如下格式："1:123;2:123"</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool AddProp(string uid, string prop)
        {
            List<string> list = new List<String>();
            CommonUtil.splitStr(prop, list, ';');
            for (int i = 0; i < list.Count; i++)
            {
                string[] strings = list[i].Split(':');
                int propId = Convert.ToInt32(strings[0]);
                int propNum = Convert.ToInt32(strings[1]);
                if (!ChangeProp(uid, propId, propNum)) return false;
            }
            return true;
        }

        /// <summary>
        /// 1.是金币,2.是话费，4徽章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="propId">1,2...</param>
        /// <param name="propNum"></param>
        /// <returns></returns>
        public static bool ChangeProp(string uid, int propId, int propNum)
        {
            UserInfo userInfo = MySqlService.userInfoManager.GetByUid(uid);
            if (userInfo == null)
            {
                MySqlService.log.Warn("用户不存在");
                return false;
            }

            if (propId == 1)
            {
                userInfo.Gold += propNum;
                if (userInfo.Gold < 0)
                {
                    User user = MySqlService.userManager.GetByUid(uid);
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
                    if (!MySqlService.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn("添加金币失败");
                        return false;
                    }
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
                    if (!MySqlService.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn("添加元宝失败");
                        return false;
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
                    if (!MySqlService.userInfoManager.Update(userInfo))
                    {
                        MySqlService.log.Warn($"添加徽章失败-medal:{userInfo.Medel}-改变的数量:{propNum}");
                        return false;
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
                UserProp userProp = MySqlService.userPropManager.GetUserProp(uid, propId);
                if (userProp == null)
                {
                    userProp = new UserProp()
                    {
                        Uid = uid,
                        PropId = propId,
                        PropNum = propNum
                    };
                    if (!MySqlService.userPropManager.Add(userProp))
                    {
                        MySqlService.log.Warn($"添加道具失败-propId：{propId}-propNum：{propNum}");
                        return false;
                    }
                }
                else
                {
                    userProp.PropNum += propNum;
                    if (userProp.PropNum >= 0)
                    {
                        if (!MySqlService.userPropManager.Update(userProp))
                        {
                            MySqlService.log.Warn("更新道具失败");
                            return false;
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
    }
}