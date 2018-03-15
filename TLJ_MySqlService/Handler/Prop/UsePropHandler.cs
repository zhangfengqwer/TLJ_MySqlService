using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL;
using NhInterMySQL.Manager;
using TLJ_MySqlService.Model;
using TLJCommon;
using TLJ_MySqlService.Utils;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UseProp)]
    class UsePropHandler : BaseHandler
    {
        private static readonly object Locker = new object();

        public override string OnResponse(string data) 
        {
            UsePropReq defaultReqData;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<UsePropReq>(data);
            }
            catch (Exception)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;
            string Uid = defaultReqData.uid;
            int propId = defaultReqData.prop_id;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(Uid))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            lock (Locker)
            {
                UsePropSql(Uid, propId, _responseData);
            }
            return _responseData.ToString();
        }

        private void UsePropSql(string uid, int propId, JObject responseData)
        {
            UserProp userProp = NHibernateHelper.userPropManager.GetUserProp(uid, propId);
            if (userProp == null || userProp.PropNum <= 0)
            {
                MySqlService.log.Warn("没有该道具或者不能使用该道具");
                OperatorFail(responseData);
            }
            else
            {
                userProp.PropNum--;
                if (NHibernateHelper.userPropManager.Update(userProp))
                {
                    UserGame userGame = NHibernateHelper.userGameManager.GetByUid(uid);
                    if (userGame == null)
                    {
                        MySqlService.log.Warn("查找用户游戏数据失败");
                        OperatorFail(responseData);
                        return;
                    }

                    switch (userProp.PropId)
                    {
                        //魅力值修正卡
                        case 109:
                            userGame.MeiliZhi = 0;
                            NHibernateHelper.userGameManager.Update(userGame);
                            break;
                        //逃跑率清零卡
                        case 108:
                            userGame.RunCount = 0;
                            NHibernateHelper.userGameManager.Update(userGame);
                            break;

                        //记牌器
                        case 101:
                        //加倍卡
                        case 102:
                        //出牌发光
                        case 105:
                            userProp.BuffNum++;
                            NHibernateHelper.userPropManager.Update(userProp);
                            break;

                        case 111:
                        case 112:
                            
                            break;
                        case 130:
                            List<JDCard> tenJDCard = MySqlManager<JDCard>.Instance.GetByPorperty("price", "10", "state", "1");
                            JDCard jDCard = null;
                            foreach (var card in tenJDCard)
                            {
                                if (card.Uid == uid)
                                {
                                    jDCard = card;
                                    break;
                                }
                            }

                            if (jDCard == null)
                            {
                                OperatorFail(responseData);
                                return;
                            }
                            else
                            {
                                jDCard.state = 2 + "";
                                MySqlManager<JDCard>.Instance.Update(jDCard);

                                SendEmailUtil.SendEmail(uid, "10元京东卡",
                                    $"您的京东卡:\n卡号:{jDCard.card_number} \n卡密:{jDCard.card_secret} \n有效期:{jDCard.valid_time}","");
                            }

                            break;
                        case 131:
                            List<JDCard> twentyJDCard = MySqlManager<JDCard>.Instance.GetByPorperty("price", "20", "state", "1");
                            JDCard jDCard2 = null;
                            foreach (var card in twentyJDCard)
                            {
                                if (card.Uid == uid)
                                {
                                    jDCard2 = card;
                                    break;
                                }
                            }

                            if (jDCard2 == null)
                            {
                                OperatorFail(responseData);
                                return;
                            }
                            else
                            {
                                jDCard2.state = 2 + "";
                                MySqlManager<JDCard>.Instance.Update(jDCard2);
                                SendEmailUtil.SendEmail(uid, "20元京东卡",
                                    $"您的京东卡:\n卡号:{jDCard2.card_number} \n卡密:{jDCard2.card_secret} \n有效期:{jDCard2.valid_time}", "");
                            }
                            break;
                    }
                    OperatorSuccess(responseData);
                }
                else
                {
                    MySqlService.log.Warn("使用道具失败");
                    OperatorFail(responseData);
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}