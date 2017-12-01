using NhInterMySQL;
using System;

public class SendEmailUtil
{
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