using NhInterMySQL;
using System;

public class SendEmailUtil
{

    /// <summary>
    /// 发邮件
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="title">标题</param>
    /// <param name="content">内容</param>
    /// <param name="reward">奖励</param>
    public static void SendEmail(string uid, string title, string content, string reward)
    {
        var sql = $"INSERT INTO user_email (uid, title, content,reward,state)" +
                  $" VALUES ('{uid}', '{title}','{content}','{reward}','0')";

        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    session.CreateSQLQuery(sql).ExecuteUpdate();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}