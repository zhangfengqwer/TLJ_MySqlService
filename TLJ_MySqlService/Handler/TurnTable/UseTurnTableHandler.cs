﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NhInterMySQL;
using NhInterMySQL.Model;
using System;
using System.Collections.Generic;
using TLJ_MySqlService.Model;
using TLJCommon;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_UseTurntable)]
    class UseTurnTableHandler : BaseHandler
    {
        public override string OnResponse(string data)
        {
            CommonReq defaultReq = null;
            try
            {
                defaultReq = JsonConvert.DeserializeObject<CommonReq>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误:" + e);
                return null;
            }
            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            var uid = defaultReq.uid;
            var type = defaultReq.type;

            if (string.IsNullOrWhiteSpace(Tag) || string.IsNullOrWhiteSpace(uid) || type < 1 || type > 2)
            {
                MySqlService.log.Warn("字段有空:" + uid);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add(MyCommon.UID, uid);
            _responseData.Add("type", type);

            //得到pvp数据
            UseTurnTableDataSql(uid, type, _responseData);
            return _responseData.ToString();
        }

        private void UseTurnTableDataSql(string uid, int type, JObject responseData)
        {
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);

            bool isSuccess = false;
            int subHuiZhangNum = 0;
            int reward = GetProbabilityReward();
            if (type == 1)
            {
                if (userInfo.freeCount <= 0)
                {
                    MySqlService.log.Warn($"{uid},转盘免费使用次数不足,当前{userInfo.freeCount}");
                    isSuccess = false;
                }
                else
                {
                    if (userInfo.luckyValue >= 98)
                    {
                        reward = 10;
                        userInfo.luckyValue = 0;
                    }
                    userInfo.freeCount--;
                    userInfo.luckyValue++;
                    if (NHibernateHelper.userInfoManager.Update(userInfo))
                    {
                        TurnTable turnTable = MySqlService.TurnTables[reward - 1];
                        MySqlService.log.Info($"{uid} 增加转盘奖励{turnTable.reward}");
                        bool addProp = MySqlUtil.AddProp(uid, turnTable.reward);
                        if (addProp)
                        {
                            isSuccess = true;
                        }
                        else
                        {
                            isSuccess = false;
                        }
                    }
                }
            }
            else if (type == 2)
            {
                if (userInfo.huizhangCount > 0)
                {
                    if (userInfo.huizhangCount == 3)
                    {
                        subHuiZhangNum = 3;
                    }
                    else if (userInfo.huizhangCount == 2)
                    {
                        subHuiZhangNum = 5;
                    }
                    else if (userInfo.huizhangCount == 1)
                    {
                        subHuiZhangNum = 10;
                    }

                    if (userInfo.Medel >= subHuiZhangNum)
                    {
                        if (userInfo.luckyValue >= 98)
                        {
                            reward = 10;
                            userInfo.luckyValue = 0;
                        }
                        userInfo.huizhangCount--;
                        userInfo.Medel -= subHuiZhangNum;
                        userInfo.luckyValue++;
                        if (NHibernateHelper.userInfoManager.Update(userInfo))
                        {
                            TurnTable turnTable = MySqlService.TurnTables[reward - 1];
                            MySqlService.log.Info($"{uid} 增加转盘奖励{turnTable.reward}");
                            bool addProp = MySqlUtil.AddProp(uid, turnTable.reward);
                            if (addProp)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }
                        }
                    }
                }
                else
                {
                    MySqlService.log.Warn($"{uid},转盘徽章使用次数不足,当前{userInfo.huizhangCount}");
                }
            }

            if (isSuccess)
            {
                OperatorSuccess(uid, reward, subHuiZhangNum, userInfo.NickName, responseData);
            }
            else
            {
                OperatorFail(responseData);
            }
        }

        private void OperatorSuccess(string uid, int reward, int subHuiZhangNum, string name, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_OK);
            responseData.Add("reward_id", reward);
            responseData.Add("subHuiZhangNum", subHuiZhangNum);
            responseData.Add("name", name);
            //得到转盘次数
            UserInfo userInfo = NHibernateHelper.userInfoManager.GetByUid(uid);
            var turnTableJsonObject = new TurnTableJsonObject();
            turnTableJsonObject.freeCount = userInfo.freeCount;
            turnTableJsonObject.huizhangCount = userInfo.huizhangCount;
            turnTableJsonObject.luckyValue = userInfo.luckyValue;
            responseData.Add("turntableData", JsonConvert.SerializeObject(turnTableJsonObject));
        }


        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }

        private static int GetProbabilityReward()
        {
            var probability1 = 32.6 * 100;
            var probability2 = 10.15 * 100;
            var probability3 = 5.89 * 100;
            var probability4 = 1 * 100;
            var probability5 = 25.49 * 100;
            var probability6 = 6 * 100;
            var probability7 = 9.6 * 100;
            var probability8 = 6.9 * 100;
            var probability9 = 2.32 * 100;
            var probability10 = 0.05 * 100;
            var doubles = new List<double>();

            doubles.Add(probability1);
            doubles.Add(probability2);
            doubles.Add(probability3);
            doubles.Add(probability4);
            doubles.Add(probability5);
            doubles.Add(probability6);
            doubles.Add(probability7);
            doubles.Add(probability8);
            doubles.Add(probability9);
            doubles.Add(probability10);

            var list = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                double temp = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    temp += doubles[j];
                }
                list.Add(temp);
            }

            int next = new Random(CommonUtil.GetRandomSeed()).Next(1, 10001);
            int num = 0;
            if (next <= list[0]) num = 1;
            else if (next <= list[1]) num = 2;
            else if (next <= list[2]) num = 3;
            else if (next <= list[3]) num = 4;
            else if (next <= list[4]) num = 5;
            else if (next <= list[5]) num = 6;
            else if (next <= list[6]) num = 7;
            else if (next <= list[7]) num = 8;
            else if (next <= list[8]) num = 9;
            else if (next <= list[9]) num = 10;

            return num;
        }
    }
}