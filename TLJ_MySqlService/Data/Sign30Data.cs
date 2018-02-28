using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

public class Sign30Data
{
    public static Sign30Data s_instance = null;

    public List<Sign30DataContent> m_sign30DataContentList = new List<Sign30DataContent>();

    public static Sign30Data getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new Sign30Data();
        }

        return s_instance;
    }

    public bool  init()
    {
        try
        {
            // 读取文件
            {
                StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "SignReward_30.json");
                string str = sr.ReadToEnd().ToString();
                sr.Close();

                initJson(str);
            }

            return true;
        }
        catch (Exception ex)
        {
            LogUtil.getInstance().writeLogToLocalNow("读取SignReward_30文件出错：" + ex.Message);

            return false;
        }
    }

    public void initJson(string json)
    {
        m_sign30DataContentList.Clear();
        
        JArray list = (JArray)JsonConvert.DeserializeObject(json);

        for (int i = 0; i < list.Count; i++)
        {
            Sign30DataContent temp = new Sign30DataContent();

            temp.id = (int)list[i]["id"];
            temp.type = (int)list[i]["type"];
            temp.day = (int)list[i]["day"];
            temp.reward_name = (string)list[i]["reward_name"];
            temp.reward_prop = (string)list[i]["reward_prop"];

            m_sign30DataContentList.Add(temp);
        }
    }

    public List<Sign30DataContent> getSign30DataContentList()
    {
        return m_sign30DataContentList;
    }

    public Sign30DataContent getSign30DataById(int id)
    {
        Sign30DataContent data = null;
        for (int i = 0; i < m_sign30DataContentList.Count; i++)
        {
            if (m_sign30DataContentList[i].id == id)
            {
                data = m_sign30DataContentList[i];
                break;
            }
        }

        return data;
    }
}

public class Sign30DataContent
{
    public int id;
    public int type;                  // 1:普通签到  2:累计签到
    public int day;
    public string reward_name = "";
    public string reward_prop = "";
}
