namespace Tank.Request
{
    using Road.Flash;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using zlib;

    public class StaticFunction
    {
        public static byte[] Compress(string str)
        {
            return Compress(Encoding.UTF8.GetBytes(str));
        }

        public static byte[] Compress(byte[] src)
        {
            return Compress(src, 0, src.Length);
        }

        public static byte[] Compress(byte[] src, int offset, int length)
        {
            MemoryStream stream = new MemoryStream();
            Stream stream2 = new ZOutputStream(stream, 9);
            stream2.Write(src, offset, length);
            stream2.Close();
            return stream.ToArray();
        }

        public static string Uncompress(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Encoding.UTF8.GetString(Uncompress(bytes));
        }

        public static byte[] Uncompress(byte[] src)
        {
            MemoryStream stream = new MemoryStream();
            Stream stream2 = new ZOutputStream(stream);
            stream2.Write(src, 0, src.Length);
            stream2.Close();
            return stream.ToArray();
        }

        public static RSACryptoServiceProvider RsaCryptor
        {
            get
            {
                string privateKey = ConfigurationSettings.AppSettings["privateKey"];
                return CryptoHelper.GetRSACrypto(privateKey);
            }
        }
    }
}

