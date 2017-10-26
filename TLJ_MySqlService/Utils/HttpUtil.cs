﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TLJ_MySqlService.Utils
{
    public class HttpUtil
    {
        //body是要传递的参数,格式"roleId=1&uid=2"
        //post的cotentType填写:
        //"application/x-www-form-urlencoded"
        //soap填写:"text/xml; charset=utf-8"
        public static string PostHttp(string url, string body)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 10000;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        public static string GetHttp(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);

            httpWebRequest.ContentType = "text/xml; charset=utf-8";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 10000;

            HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        public static string SendSms(string uid, string phoneNum)
        {
            string url = "http://servicesy.51v.cn/partnerws/SmsService.asmx/SendSms";
            string getBody = "?userId=" + uid + "&cellPhoneNum=" + phoneNum + "&keyStr=sy123";
            MySqlService.log.Info(url + getBody);
            return GetHttp(url + getBody);
        }

        public static string CheckSms(string uid, string phoneNum, string verificationCode)
        {
            string url = "http://servicesy.51v.cn/partnerws/SmsService.asmx/CheckSmsToJson";
            string getBody = "?userId=" + uid + "&cellPhoneNum=" + phoneNum + "&verificationCode=" + verificationCode +
                             "&keyStr=sy123";
            return GetHttp(url + getBody);
        }
    }
}