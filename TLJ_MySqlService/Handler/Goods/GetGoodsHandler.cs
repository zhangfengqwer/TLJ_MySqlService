using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLJCommon;
using Zfstu.Manager;
using Zfstu.Model;

namespace TLJ_MySqlService.Handler
{
    class GetGoodsHandler : BaseHandler
    {
        private static MySqlManager<Goods> goodsManager = new MySqlManager<Goods>();

        public GetGoodsHandler()
        {
            tag = Consts.Tag_GetShop;
        }

        public override string OnResponse(string data)
        {
            DefaultReqData defaultReqData = null;
            try
            {
                defaultReqData = JsonConvert.DeserializeObject<DefaultReqData>(data);
            }
            catch (Exception e)
            {
                MySqlService.log.Warn("传入的参数有误");
                return null;
            }
            string Tag = defaultReqData.tag;
            int ConnId = defaultReqData.connId;

            if (string.IsNullOrWhiteSpace(Tag))
            {
                MySqlService.log.Warn("字段有空");
                return null;
            }
            //传给客户端的数据
            JObject _responseData = new JObject();
            _responseData.Add(MyCommon.TAG, Tag);
            _responseData.Add(MyCommon.CONNID, ConnId);

            GetGoodsSql(_responseData);
            return _responseData.ToString();
        }

        private void GetGoodsSql(JObject responseData)
        {
            List<Goods> goods = goodsManager.GetAll().ToList();
            OperatorSuccess(goods,responseData);
        }

        //数据库操作成功
        private void OperatorSuccess(List<Goods> goods, JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_OK);
            responseData.Add("shop_list", JsonConvert.SerializeObject(goods));
        }

        //数据库操作失败
        private void OperatorFail(JObject responseData)
        {
            responseData.Add(MyCommon.CODE, (int)Consts.Code.Code_CommonFail);
        }
    }
}