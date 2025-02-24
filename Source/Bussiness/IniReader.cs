namespace Bussiness
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class IniReader
    {
        private string FilePath;

        public IniReader(string _FilePath)
        {
            this.FilePath = _FilePath;
        }

        public string GetIniString(string Section, string Key)
        {
            StringBuilder retVal = new StringBuilder(0x9f6);
            GetPrivateProfileString(Section, Key, "", retVal, 0x9f6, this.FilePath);
            return retVal.ToString();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}

