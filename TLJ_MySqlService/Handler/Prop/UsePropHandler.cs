using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using NhInterMySQL;
using TLJ_MySqlService.Model;
using TLJCommon;
using TLJ_MySqlService.Utils;
using NhInterMySQL.Model;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UseProp)]
    class UsePropHandler : BaseHandler
    {
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

            UsePropSql(Uid, propId, _responseData);
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