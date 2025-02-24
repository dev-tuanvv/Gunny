namespace Tank.Request
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    public class WebsResponse
    {
        public static string GetPage(string url, string postData, string encodeType, out string err)
        {
            Stream requestStream = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.GetEncoding(encodeType);
            byte[] bytes = encoding.GetBytes(postData);
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer container = new CookieContainer();
                request.CookieContainer = container;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                response = request.GetResponse() as HttpWebResponse;
                string str = new StreamReader(response.GetResponseStream(), encoding).ReadToEnd();
                err = string.Empty;
                return str;
            }
            catch (Exception exception)
            {
                err = exception.Message;
                return string.Empty;
            }
        }
    }
}

