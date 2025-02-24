namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    public class csFunction
    {
        private static string[] al;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static csFunction()
        {
            char[] separator = new char[] { '|' };
            al = ";|and|1=1|exec|insert|select|delete|update|like|count|chr|mid|master|or|truncate|char|declare|join".Split(separator);
        }

        public static string BuildCelebConsortia(string file, int order)
        {
            return BuildCelebConsortia(file, order, "");
        }

        public static string BuildCelebConsortia(string file, int order, string fileNotCompress)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            int num = 0;
            try
            {
                int num2 = 1;
                int num3 = 50;
                int num4 = -1;
                string str2 = "";
                int num5 = -1;
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaInfo[] infoArray = bussiness.GetConsortiaPage(num2, num3, ref num, order, str2, num4, num5, -1);
                    foreach (ConsortiaInfo info in infoArray)
                    {
                        XElement content = FlashUtils.CreateConsortiaInfo(info);
                        if (info.ChairmanID > 0)
                        {
                            using (PlayerBussiness bussiness2 = new PlayerBussiness())
                            {
                                PlayerInfo userSingleByUserID = bussiness2.GetUserSingleByUserID(info.ChairmanID);
                                if (userSingleByUserID != null)
                                {
                                    content.Add(FlashUtils.CreateCelebInfo(userSingleByUserID));
                                }
                            }
                        }
                        result.Add(content);
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error(file + " is fail!", exception);
            }
            result.Add(new XAttribute("total", num));
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrEmpty(fileNotCompress))
            {
                CreateCompressXml(result, fileNotCompress, false);
            }
            return CreateCompressXml(result, file, true);
        }

        public static string BuildCelebConsortiaFightPower(string file, string fileNotCompress)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            int length = 0;
            try
            {
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    //ConsortiaInfo[] infoArray = bussiness.UpdateConsortiaFightPower();//bt90 bo
                    //length = infoArray.Length;
                    //foreach (ConsortiaInfo info in infoArray)
                    //{
                    //    XElement content = FlashUtils.CreateConsortiaInfo(info);
                    //    if (info.ChairmanID > 0)
                    //    {
                    //        using (PlayerBussiness bussiness2 = new PlayerBussiness())
                    //        {
                    //            PlayerInfo userSingleByUserID = bussiness2.GetUserSingleByUserID(info.ChairmanID);
                    //            if (userSingleByUserID != null)
                    //            {
                    //                content.Add(FlashUtils.CreateCelebInfo(userSingleByUserID));
                    //            }
                    //        }
                    //    }
                    //    result.Add(content);
                    //}
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error(file + " is fail!", exception);
            }
            result.Add(new XAttribute("total", length));
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrEmpty(fileNotCompress))
            {
                CreateCompressXml(result, fileNotCompress, false);
            }
            return CreateCompressXml(result, file, true);
        }

        public static string BuildCelebUsers(string file, int order)
        {
            return BuildCelebUsers(file, order, "");
        }

        public static string BuildCelebUsers(string file, int order, string fileNotCompress)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                int num = 1;
                int num2 = 50;
                int num3 = -1;
                int num4 = 0;
                bool flag2 = false;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                   // bussiness.UpdateUserReputeFightPower();//bt90 bo
                    PlayerInfo[] infoArray = bussiness.GetPlayerPage(num, num2, ref num4, order, num3, ref flag2);
                    if (flag2)
                    {
                        foreach (PlayerInfo info in infoArray)
                        {
                            result.Add(FlashUtils.CreateCelebInfo(info));
                        }
                        flag = true;
                        str = "Success!";
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(file + " is fail!", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrEmpty(fileNotCompress))
            {
                CreateCompressXml(result, fileNotCompress, false);
            }
            return CreateCompressXml(result, file, true);
        }

        public static string ConvertSql(string inputString)
        {
            inputString = inputString.Trim().ToLower();
            inputString = inputString.Replace("'", "''");
            inputString = inputString.Replace(";--", "");
            inputString = inputString.Replace("=", "");
            inputString = inputString.Replace(" or", "");
            inputString = inputString.Replace(" or ", "");
            inputString = inputString.Replace(" and", "");
            inputString = inputString.Replace("and ", "");
            if (!SqlChar(inputString))
            {
                inputString = "";
            }
            return inputString;
        }

        public static string CreateCompressXml(XElement result, string file, bool isCompress)
        {
            return CreateCompressXml(StaticsMgr.CurrentPath, result, file, isCompress);
        }

        public static string CreateCompressXml(string path, XElement result, string file, bool isCompress)
        {
            try
            {
                file = file + ".xml";
                path = Path.Combine(path, file);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    if (isCompress)
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(StaticFunction.Compress(XmlExtends.ToString(result, false)));
                        }
                    }
                    else
                    {
                        using (StreamWriter writer2 = new StreamWriter(stream))
                        {
                            writer2.Write(XmlExtends.ToString(result, false));
                        }
                    }
                }
                return ("Build:" + file + ",Success!");
            }
            catch (Exception exception)
            {
                log.Error("CreateCompressXml " + file + " is fail!", exception);
                return ("Build:" + file + ",Fail!");
            }
        }

        public static string CreateCompressXml(HttpContext context, XElement result, string file, bool isCompress)
        {
            string path = context.Server.MapPath("~");
            return CreateCompressXml(path, result, file, isCompress);
        }

        public static bool SqlChar(string v)
        {
            if (v.Trim() != "")
            {
                foreach (string str in al)
                {
                    if ((v.IndexOf(str + " ") > -1) || (v.IndexOf(" " + str) > -1))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ValidAdminIP(string ip)
        {
            return true;
            string getAdminIP = GetAdminIP;
            return (string.IsNullOrEmpty(getAdminIP) || getAdminIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetAdminIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["AdminIP"];
            }
        }
    }
}

