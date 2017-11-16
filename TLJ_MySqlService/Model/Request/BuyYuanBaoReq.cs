public class BuyYuanBaoReq
{
    public override string ToString()
    {
        return $"{nameof(tag)}: {tag}, {nameof(connId)}: {connId}, {nameof(uid)}: {uid}, {nameof(order_id)}: {order_id}, {nameof(account)}: {account}, {nameof(password)}: {password}, {nameof(goods_id)}: {goods_id}, {nameof(goods_num)}: {goods_num}, {nameof(price)}: {price}";
    }

    public string tag;
    public int connId;
    public string uid;
    public string order_id;
    public string account;
    public string password;
    public int goods_id;
    public int goods_num;
    public float price;

}