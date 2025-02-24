namespace Bussiness
{
    using log4net;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class LanguageMgr
    {
        private static Hashtable LangsSentences = new Hashtable();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetTranslation(string translateId, params object[] args)
        {
            if (LangsSentences.ContainsKey(translateId))
            {
                string format = (string) LangsSentences[translateId];
                try
                {
                    format = string.Format(format, args);
                }
                catch (Exception exception)
                {
                    log.Error(string.Concat(new object[] { "Parameters number error, ID: ", translateId, " (Arg count=", args.Length, ")" }), exception);
                }
                if (format != null)
                {
                    return format;
                }
            }
            return translateId;
        }

        private static Hashtable LoadLanguage(string path)
        {
            Hashtable hashtable = new Hashtable();
            string str = path + LanguageFile;
            if (!File.Exists(str))
            {
                log.Error("Language file : " + str + " not found !");
                return hashtable;
            }
            IList list = new ArrayList(File.ReadAllLines(str, Encoding.UTF8));
            foreach (string str2 in list)
            {
                if (!(str2.StartsWith("#") || (str2.IndexOf(':') == -1)))
                {
                    string[] strArray2 = new string[] { str2.Substring(0, str2.IndexOf(':')), str2.Substring(str2.IndexOf(':') + 1) };
                    strArray2[1] = strArray2[1].Replace("\t", "");
                    hashtable[strArray2[0]] = strArray2[1];
                }
            }
            return hashtable;
        }

        public static bool Reload(string path)
        {
            try
            {
                Hashtable hashtable = LoadLanguage(path);
                if (hashtable.Count > 0)
                {
                    Interlocked.Exchange<Hashtable>(ref LangsSentences, hashtable);
                    return true;
                }
            }
            catch (Exception exception)
            {
                log.Error("Load language file error:", exception);
            }
            return false;
        }

        public static bool Setup(string path)
        {
            return Reload(path);
        }

        private static string LanguageFile
        {
            get
            {
                return ConfigurationManager.AppSettings["LanguagePath"];
            }
        }
    }
}

