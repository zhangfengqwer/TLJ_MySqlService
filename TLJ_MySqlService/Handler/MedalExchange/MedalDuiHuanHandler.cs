using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NhInterMySQL.Manager;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_MedalDuiHuan)]
    class MedalDuiHuanHandler : BaseHandler
    {
        private static readonly object Locker = new object();
        public override string OnResponse(string data)
        {
            BindExtendCode bindExtendCode = null;
            try
            {
                bindExtendCode = JsonConvert.DeserializeObject<BindExtendCode>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误：" + e);
                return null;
            }

            string _tag = bindExtendCode.tag;
            string uid = bindExtendCode.uid;
            int _connId = bindExtendCode.connId;
            int goods_id = bindExtendCode.goods_id;
            int num = bindExtendCode.num;

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(uid) || num != 1 || goods_id == 0)
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            lock(Locker)
            {
                MedalExchange(uid, goods_id, num, _responseData);
            }

            return _responseData.ToString();
        }

        private void MedalExchange(string uid, int goodsId, int num, JObject responseData)
        {
            try
            {
                MedalExchargeRewardData exchargeRewardData = null;
                foreach (var medalExchargeRewardData in MySqlService.medalExchargeRewardDatas)
                {
                    if (medalExchargeRewardData.goods_id == goodsId)
                    {
                        exchargeRewardData = medalExchargeRewardData;
                        break;
                    }
                }

                if (exchargeRewardData == null)
                {
                    OperatorFail(responseData, "goodsId：" + goodsId);
                    return;
                }

                int cost = exchargeRewardData.price * num;
                UserInfo userInfo = MySqlManager<UserInfo>.Instance.GetByUid(uid);
                if (userInfo.Medel < cost)
                {
                    OperatorFail(responseData, "徽章不足");
                }
                else
                {
                    //徽章限额

                    List<UserExchange> medalLimitMonth = MySqlManager<UserExchange>.Instance.GetMedalLimitMonth(uid);
                    int total = 0;
                    foreach (var exchange in medalLimitMonth)
                    {
                        total += exchange.medal_cost;
                    }

                    if (total >= 300)
                    {
                        OperatorFail(responseData, $"当月已达限额300,已花费{total}");
                        return;
                    }

                    UserInfo info = MySqlManager<UserInfo>.Instance.GetByUid(uid);

                    int vipLevel = VipUtil.GetVipLevel(info.RechargeVip);

                    if (vipLevel < exchargeRewardData.vipLevel)
                    {
                        OperatorFail(responseData, "贵族等级不足");
                        return;
                    }

                    string reward = $"110:{-cost}";

                    string curYearMonth = CommonUtil.getCurYearMonth();

                    string day = curYearMonth + "/" + DateTime.Now.Day;

                    List<JDCard> tenJDCard = MySqlManager<JDCard>.Instance.GetByPorperty("price", "10", "state", "0");
                    List<JDCard> twentyJDCard = MySqlManager<JDCard>.Instance.GetByPorperty("price", "20", "state", "0");

                    //判断是否兑换京东卡
                    if (exchargeRewardData.goods_id == 4)
                    {

                        if (tenJDCard.Count == 0)
                        {
                            OperatorFail(responseData, "10元京东卡数量不足");
                            return;
                        }
                    }

                    if (exchargeRewardData.goods_id == 5)
                    {
                        if (twentyJDCard.Count == 0)
                        {
                            OperatorFail(responseData, "20元京东卡数量不足");
                            return;
                        }
                    }

                    if (MySqlManager<UserExchange>.Instance.Add(new UserExchange()
                    {
                        Uid = uid,
                        goods_id = goodsId,
                        num = num,
                        day = day,
                        medal_cost = cost,
                        create_time = DateTime.Now
                    }))
                    {
                        for (int i = 0; i < num; i++)
                        {
                            MySqlUtil.AddProp(uid, exchargeRewardData.reward_prop, "徽章兑换话费");
                        }

                        if (MySqlUtil.AddProp(uid, reward, "徽章兑换话费"))
                        {
                            responseData.Add("reward", exchargeRewardData.reward_prop);
                            OperatorSuccess(responseData, "兑换成功");

                            switch (exchargeRewardData.goods_id)
                            {
                                case 4:
                                    tenJDCard[0].state = 1 + "";
                                    tenJDCard[0].Uid = uid;
                                    MySqlManager<JDCard>.Instance.Update(tenJDCard[0]);
                                    break;
                                case 5:
                                    twentyJDCard[0].state = 1 + "";
                                    twentyJDCard[0].Uid = uid;
                                    MySqlManager<JDCard>.Instance.Update(twentyJDCard[0]);
                                    break;
                            }
                        }
                        else
                        {
                            OperatorFail(responseData, "该道具已兑换");
                        }
                    }
                    else
                    {
                        OperatorFail(responseData, "该道具已兑换");
                    }
                }
            }
            catch (Exception e)
            {
                OperatorFail(responseData, "goodsId：" + goodsId);
                MySqlService.log.Warn(e);
            }
        }

        private void OperatorSuccess(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("msg", msg);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}