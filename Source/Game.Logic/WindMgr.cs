namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;
    using System.Reflection;

    public class WindMgr
    {
        private static Dictionary<int, WindInfo> _winds;
        private static readonly Color[] c = new Color[] { Color.Yellow, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Aqua, Color.DarkCyan, Color.Purple };
        private static readonly int[] CategoryID = new int[] { 0x3e9, 0x3ea, 0x3eb, 0x3ec, 0x3ed, 0x3ed, 0x3ef, 0x3f0, 0x3f1 };
        private static readonly string[] font = new string[] { "Comic Sans MS" };
        private static readonly string[] fontWind = new string[] { "•", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;
        private static readonly int[] WindID = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public static byte[] CreateVane(string wind)
        {
            byte[] buffer;
            int maxValue = 1;
            int width = 0x12;
            if (isSmall(wind))
            {
                width = 10;
            }
            Bitmap image = new Bitmap(width, 0x20);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            try
            {
                graphics.Clear(Color.Transparent);
                int num3 = rand.Next(7);
                Brush brush = new SolidBrush(Color.Red);
                StringFormat format = new StringFormat(StringFormatFlags.NoClip) {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                int index = rand.Next(WindMgr.font.Length);
                Font font = new Font(WindMgr.font[index], 17f, FontStyle.Italic);
                Point point = new Point(8, 12);
                if (isSmall(wind))
                {
                    if (wind == fontWind[0])
                    {
                        font = new Font(WindMgr.font[index], 10f, FontStyle.Regular);
                    }
                    point = new Point(4, 0x10);
                }
                float angle = ThreadSafeRandom.NextStatic(-maxValue, maxValue);
                graphics.TranslateTransform((float) point.X, (float) point.Y);
                graphics.RotateTransform(angle);
                graphics.DrawString(wind.ToString(), font, brush, 1f, 1f, format);
                graphics.RotateTransform(-angle);
                graphics.TranslateTransform(2f, -((float) point.Y));
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                buffer = stream.ToArray();
            }
            finally
            {
                graphics.Dispose();
                image.Dispose();
            }
            return buffer;
        }

        public static WindInfo FindWind(int ID)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_winds.ContainsKey(ID))
                {
                    return _winds[ID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static List<WindInfo> GetWind()
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                List<WindInfo> list = new List<WindInfo>();
                for (int i = 0; i < _winds.Values.Count; i++)
                {
                    list.Add(_winds[i]);
                }
                if (list.Count > 0)
                {
                    return list;
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static byte GetWindID(int wind, int pos)
        {
            if (wind < 10)
            {
                switch (pos)
                {
                    case 1:
                        return 10;

                    case 3:
                        return ((wind == 0) ? ((byte) 10) : ((byte) wind));
                }
            }
            if ((wind >= 10) && (wind < 20))
            {
                switch (pos)
                {
                    case 1:
                        return 1;

                    case 3:
                        return (((wind - 10) == 0) ? ((byte) 10) : ((byte) (wind - 10)));
                }
            }
            if ((wind >= 20) && (wind < 30))
            {
                switch (pos)
                {
                    case 1:
                        return 2;

                    case 3:
                        return (((wind - 20) == 0) ? ((byte) 10) : ((byte) (wind - 20)));
                }
            }
            if ((wind >= 30) && (wind < 40))
            {
                switch (pos)
                {
                    case 1:
                        return 3;

                    case 3:
                        return (((wind - 30) == 0) ? ((byte) 10) : ((byte) (wind - 30)));
                }
            }
            if ((wind >= 40) && (wind < 50))
            {
                switch (pos)
                {
                    case 1:
                        return 4;

                    case 3:
                        return (((wind - 40) == 0) ? ((byte) 10) : ((byte) (wind - 40)));
                }
            }
            return 0;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _winds = new Dictionary<int, WindInfo>();
                rand = new ThreadSafeRandom();
                return LoadWinds(_winds);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("WindInfoMgr", exception);
                }
                return false;
            }
        }

        public static bool isSmall(string wind)
        {
            return ((wind == fontWind[0]) || (wind == fontWind[1]));
        }

        private static bool LoadWinds(Dictionary<int, WindInfo> Winds)
        {
            foreach (int num2 in WindID)
            {
                WindInfo info = new WindInfo();
                byte[] buffer = CreateVane(fontWind[num2]);
                if ((buffer == null) || (buffer.Length <= 0))
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("Load Wind Error!");
                    }
                    return false;
                }
                info.WindID = num2;
                info.WindPic = buffer;
                if (!Winds.ContainsKey(num2))
                {
                    Winds.Add(num2, info);
                }
            }
            return true;
        }

        public static byte[] ReadImageFile(string imageLocation)
        {
            FileInfo info = new FileInfo(imageLocation);
            long length = info.Length;
            FileStream input = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(input);
            return reader.ReadBytes((int) length);
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, WindInfo> winds = new Dictionary<int, WindInfo>();
                if (LoadWinds(winds))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _winds = winds;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("WindMgr", exception);
                }
            }
            return false;
        }
    }
}

