namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class IMFriendsBbs : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            IAgentFriends friends = new Normal();
            StringBuilder builder = new StringBuilder();
            HttpContext current = HttpContext.Current;
            string s = friends.FriendsString(current.Request.Params["Uid"]);
            DataSet set = new DataSet();
            if (s != "")
            {
                try
                {
                    set.ReadXml(new StringReader(s));
                    for (int i = 0; i < set.Tables["item"].DefaultView.Count; i++)
                    {
                        builder.Append(set.Tables["item"].DefaultView[i]["UserName"].ToString() + ",");
                    }
                }
                catch (Exception exception)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Get Table Item ", exception);
                    }
                }
            }
            if ((builder.Length <= 1) || (s == ""))
            {
                element.Add(new XAttribute("value", flag));
                element.Add(new XAttribute("message", str));
                context.Response.ContentType = "text/plain";
                context.Response.Write(XmlExtends.ToString(element, false));
            }
            else
            {
                char[] separator = new char[] { ',' };
                string[] source = builder.ToString().Split(separator);
                ArrayList list = new ArrayList();
                StringBuilder builder2 = new StringBuilder(0xfa0);
                for (int j = 0; j < source.Count<string>(); j++)
                {
                    if (source[j] == "")
                    {
                        break;
                    }
                    if ((builder2.Length + source[j].Length) < 0xfa0)
                    {
                        builder2.Append(source[j] + ",");
                    }
                    else
                    {
                        list.Add(builder2.ToString());
                        builder2.Remove(0, builder2.Length);
                    }
                }
                list.Add(builder2.ToString());
                try
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        string str3 = list[k].ToString();
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            FriendInfo[] friendsBbs = bussiness.GetFriendsBbs(str3);
                            for (int m = 0; m < friendsBbs.Count<FriendInfo>(); m++)
                            {
                                DataRow[] rowArray = set.Tables["item"].Select("UserName='" + friendsBbs[m].UserName + "'");
                                object[] content = new object[] { new XAttribute("NickName", friendsBbs[m].NickName), new XAttribute("UserName", friendsBbs[m].UserName), new XAttribute("UserId", friendsBbs[m].UserID), new XAttribute("Photo", (rowArray[0]["Photo"] == null) ? "" : rowArray[0]["Photo"].ToString()), new XAttribute("PersonWeb", (rowArray[0]["PersonWeb"] == null) ? "" : rowArray[0]["PersonWeb"].ToString()), new XAttribute("IsExist", friendsBbs[m].IsExist), new XAttribute("OtherName", (rowArray[0]["OtherName"] == null) ? "" : rowArray[0]["OtherName"].ToString()) };
                                XElement element2 = new XElement("Item", content);
                                element.Add(element2);
                            }
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
                catch (Exception exception2)
                {
                    log.Error("IMFriendsGood", exception2);
                }
                element.Add(new XAttribute("value", flag));
                element.Add(new XAttribute("message", str));
                context.Response.ContentType = "text/plain";
                context.Response.Write(XmlExtends.ToString(element, false));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public interface IAgentFriends
        {
            string FriendsString(string uid);
        }

        public class Normal : IMFriendsBbs.IAgentFriends
        {
            private string Url;

            public string FriendsString(string uid)
            {
                try
                {
                    if (FriendInterface == "")
                    {
                        return string.Empty;
                    }
                    string err = "";
                    object[] args = new object[] { uid };
                    this.Url = string.Format(CultureInfo.InvariantCulture, FriendInterface, args);
                    string str2 = WebsResponse.GetPage(this.Url, "", "utf-8", out err);
                    if (err != "")
                    {
                        throw new Exception(err);
                    }
                    return str2;
                }
                catch (Exception exception)
                {
                    if (IMFriendsBbs.log.IsErrorEnabled)
                    {
                        IMFriendsBbs.log.Error("Normal：", exception);
                    }
                }
                return string.Empty;
            }

            public static string FriendInterface
            {
                get
                {
                    return ConfigurationSettings.AppSettings["FriendInterface"];
                }
            }
        }
    }
}

