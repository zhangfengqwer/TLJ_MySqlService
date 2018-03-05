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
    [Handler(Consts.Tag_MyTuiGuangYouLiData)]
    class MyTuiGuangYouLiDataHandler : BaseHandler
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
            int _connId = bindExtendCode.connId;
            string uid = bindExtendCode.uid;

            if (string.IsNullOrWhiteSpace(_tag) || string.IsNullOrWhiteSpace(uid))
            {
                MySqlService.log.Warn("字段有空:" + data);
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, _tag);
            _responseData.Add(MyCommon.CONNID, _connId);

            GetExtendData(uid,_responseData);

            return _responseData.ToString();
        }

        private void GetExtendData(string uid, JObject jObject)
        {
            IList<UserExtend> myExtendData = MySqlManager<UserExtend>.Instance.GetMyExtendDataByUid(uid);
            List<ExtendData> extendDatas = new List<ExtendData>();

            foreach (var data in myExtendData)
            {
                UserInfo userInfo = MySqlManager<UserInfo>.Instance.GetByUid(data.Uid);

                ExtendData extendData = new ExtendData()
                {
                    uid = data.Uid,
                    name = userInfo.NickName,
                    task1_state = data.task1,
                    task2_state = data.task2
                };
                extendDatas.Add(extendData);
            }
            jObject.Add("myTuiGuangYouLiDataList", JsonConvert.SerializeObject(extendDatas));
        }


        private void OperatorSuccess(JObject responseData,string msg)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
         
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData,string msg)
        {
            responseData.Add(MyCommon.CODE, (int) Consts.Code.Code_CommonFail);
        }
    }
}