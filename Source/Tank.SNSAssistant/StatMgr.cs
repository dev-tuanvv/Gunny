﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace Tank.SNSAssistant
{
    public class StatMgr
    {
        public static string LogPath
        {
            get
            {
                return ConfigurationSettings.AppSettings["LogPath"];
            }
        }

        public static bool SaveError(string error)
        {
            try
            {
                string CurrentPath = HttpContext.Current.Server.MapPath("~");
                string _record = CurrentPath + LogPath;

                if (!Directory.Exists(_record))
                {
                    Directory.CreateDirectory(_record);
                }

                string file = string.Format("{0}\\{1:yyyyMMdd}.log", _record, DateTime.Now);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(string.Format("{0},{1}",  DateTime.Now, error));
                    }
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void SaveInfo(string info)
        {
            try
            {
                string CurrentPath = HttpContext.Current.Server.MapPath("~");
                string _record = CurrentPath + LogPath;

                if (!Directory.Exists(_record))
                {
                    Directory.CreateDirectory(_record);
                }

                string file = string.Format("{0}\\{1:yyyyMMdd}_ReturnStr.txt", _record, DateTime.Now);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(string.Format("{0},{1}", DateTime.Now, info));
                    }
                }
            }
            catch
            {
            }
            
        }
    }
}
