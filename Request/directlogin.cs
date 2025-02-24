namespace Tank.Request
{
    using Bussiness.Interface;
    using System;
    using System.Configuration;
    using System.Web;

    public class directlogin : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string str2 = context.Request["username"];
            string str3 = context.Request["phpkey"];
            string str4 = Guid.NewGuid().ToString();
            string str5 = BaseInterface.ConvertDateTimeInt(DateTime.Now).ToString();
            string getLoginKey = string.Empty;
            if (str3 == this.PHP_Key)
            {
                if (string.IsNullOrEmpty(getLoginKey))
                {
                    getLoginKey = BaseInterface.GetLoginKey;
                }
                string str7 = BaseInterface.md5(str2 + str4 + str5.ToString() + getLoginKey);
                if (BaseInterface.RequestContent(BaseInterface.LoginUrl + "?content=" + HttpUtility.UrlEncode(str2 + "|" + str4 + "|" + str5.ToString() + "|" + str7)) == "0")
                {
                    context.Response.Write(str4.ToUpper());
                }
                else
                {
                    context.Response.Write("0");
                }
            }
            else
            {
                context.Response.Write("2");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string PHP_Key
        {
            get
            {
                return ConfigurationManager.AppSettings["PHP_Key"];
            }
        }
    }
}

