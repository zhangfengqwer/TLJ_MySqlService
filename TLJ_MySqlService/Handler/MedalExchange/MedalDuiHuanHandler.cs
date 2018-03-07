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

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(uid) || num == 0 || goods_id == 0)
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            MedalExchange(uid, goods_id, num, _responseData);

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
                    userInfo.Medel -= cost;
                    if (MySqlManager<UserInfo>.Instance.Update(userInfo))
                    {
                        for (int i = 0; i < num; i++)
                        {
                            MySqlUtil.AddProp(uid, exchargeRewardData.reward_prop, "徽章兑换");
                        }

                        MySqlManager<UserExchange>.Instance.Add(new UserExchange()
                        {
                            Uid = uid,
                            goods_id = goodsId,
                            num = num,
                            create_time = DateTime.Now
                        });
                        responseData.Add("reward", exchargeRewardData.reward_prop);
                        OperatorSuccess(responseData, "兑换成功");
                    }
                    else
                    {
                        OperatorFail(responseData, "更新用户信息失败");
                    }
                }
            }
            catch (Exception e)
            {
                OperatorFail(responseData, "goodsId："+ goodsId);
                MySqlService.log.Warn(e);
            }

        }

        private void OperatorSuccess(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("msg", msg);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData, string msg)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
            responseData.Add("msg", msg);
        }
    }
}
