using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

/// <summary>
/// Xml序列化与反序列化
/// </summary>
public class XmlUtil
{
    #region 反序列化

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="xml">XML字符串</param>
    /// <returns></returns>
    public static T Deserialize<T>(string xml)
    {
        try
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                return (T) xmldes.Deserialize(sr);
            }
        }
        catch (Exception e)
        {
            throw new Exception("反序列化失败", e);
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public static T DeserializeByPath<T>(string path)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    T t = XmlUtil.Deserialize<T>(sr.ReadToEnd());
                    return t;
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static object Deserialize<T>(Stream stream)
    {
        XmlSerializer xmldes = new XmlSerializer(typeof(T));
        return xmldes.Deserialize(stream);
    }

    #endregion

    #region 序列化

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="t">对象</param>
    /// <returns></returns>
    public static string Serializer<T>(T t)
    {
        MemoryStream Stream = new MemoryStream();
        XmlSerializer xml = new XmlSerializer(typeof(T));
        try
        {
            //序列化对象
            xml.Serialize(Stream, t);
        }
        catch (InvalidOperationException)
        {
            throw;
        }

        Stream.Position = 0;
        StreamReader sr = new StreamReader(Stream);
        string str = sr.ReadToEnd();

        sr.Dispose();
        Stream.Dispose();

        return str;
    }

    /// <summary>
    /// 序列化对象，存储到文件中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string Serializer<T>(T t, string path)
    {
        MemoryStream Stream = new MemoryStream();
        XmlSerializer xml = new XmlSerializer(typeof(T));
        try
        {
            //序列化对象
            xml.Serialize(Stream, t);
        }
        catch (InvalidOperationException)
        {
            throw;
        }

        Stream.Position = 0;
        StreamReader sr = new StreamReader(Stream);
        string str = sr.ReadToEnd();

        sr.Dispose();
        Stream.Dispose();

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
        }
        return str;
    }

    #endregion
}