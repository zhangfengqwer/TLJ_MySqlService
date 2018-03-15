using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using NhInterMySQL.Manager;
using TLJ_MySqlService.Model;
using TLJCommon;
using NhInterMySQL.Model;
using TLJ_MySqlService.Utils;

namespace TLJ_MySqlService.Handler
{
    [Handler(Consts.Tag_HuaFeiSuiPianDuiHuan)]
    class ExchangeTeleFarePieceHandler : BaseHandler
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
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }

            string Tag = defaultReq.tag;
            int connId = defaultReq.connId;
            string uid = defaultReq.uid;
            int duihuanId = defaultReq.duihuan_id;

            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, connId);
            _responseData.Add("duihuan_id", duihuanId);

            //得到pvp数据

            ExchangeTeleFarePiece(uid, duihuanId, _responseData);

            return _responseData.ToString();
        }

        private void ExchangeTeleFarePiece(string uid, int duihuanId, JObject responseData)
        {
            TeleFarePieceData pieceData = null;
            foreach (var data in MySqlService.teleFarePieceDatas)
            {
                if (data.duihuan_id == duihuanId)
                {
                    pieceData = data;
                    break;
                }
            }

            if (pieceData == null)
            {
                OperatorFail(responseData, $"兑换id有误：{duihuanId}");
            }
            else
            {
                UserProp userProp = MySqlManager<UserProp>.Instance.GetUserProp(uid, pieceData.material_id);

                if (userProp == null)
                {
                    OperatorFail(responseData, $"材料不足");
                }
                else
                {
                    if (userProp.PropNum < pieceData.material_num)
                    {
                        OperatorFail(responseData, $"材料不足：{ pieceData.material_num}");
                    }
                    else
                    {
                        userProp.PropNum -= pieceData.material_num;
                        MySqlManager<UserProp>.Instance.Update(userProp);

                        string propReward = pieceData.Synthesis_id + ":" + pieceData.Synthesis_num;
                        MySqlUtil.AddProp(uid, propReward, "话费碎片兑换");
                        OperatorSuccess(responseData, "兑换成功");
                    }
                }
            }
        }

        //数据库操作成功
        private void OperatorSuccess(JObject responseData,string msg)
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