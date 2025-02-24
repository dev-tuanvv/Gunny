namespace Bussiness
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    public class CheckCode
    {
        private static Color[] c = new Color[] { Color.Gray, Color.DimGray };
        private static char[] digitals = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static string[] font = new string[] { "Verdana", "Terminal", "Comic Sans MS", "Arial", "Tekton Pro" };
        private static char[] letters = new char[] { 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 
            'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 
            'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 
            'Y', 'Z'
         };
        private static char[] lowerLetters = new char[] { 
            'a', 'b', 'c', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 
            'v', 'w', 'x', 'y', 'z'
         };
        private static char[] mix = new char[] { 
            '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'h', 'k', 
            'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 
            'D', 'E', 'F', 'G', 'H', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 
            'X', 'Y', 'Z'
         };
        public static ThreadSafeRandom rand = new ThreadSafeRandom();
        private static char[] upperLetters = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 
            'U', 'V', 'W', 'X', 'Y', 'Z'
         };

        public static byte[] CreateImage(string randomcode)
        {
            byte[] buffer;
            int maxValue = 30;
            int width = randomcode.Length * 30;
            Bitmap image = new Bitmap(width, 0x20);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            try
            {
                graphics.Clear(Color.Transparent);
                int index = rand.Next(2);
                Brush brush = new SolidBrush(c[index]);
                for (int i = 0; i < 1; i++)
                {
                    int num5 = rand.Next(image.Width / 2);
                    int num6 = rand.Next((image.Width * 3) / 4, image.Width);
                    int num7 = rand.Next(image.Height);
                    int num8 = rand.Next(image.Height);
                    graphics.DrawBezier(new Pen(c[index], 2f), (float) num5, (float) num7, (float) ((num5 + num6) / 4), 0f, (float) (((num5 + num6) * 3) / 4), (float) image.Height, (float) num6, (float) num8);
                }
                char[] chArray = randomcode.ToCharArray();
                StringFormat format = new StringFormat(StringFormatFlags.NoClip) {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                for (int j = 0; j < chArray.Length; j++)
                {
                    int num10 = rand.Next(5);
                    Font font = new Font(CheckCode.font[num10], 22f, FontStyle.Bold);
                    Point point = new Point(0x10, 0x10);
                    float angle = ThreadSafeRandom.NextStatic(-maxValue, maxValue);
                    graphics.TranslateTransform((float) point.X, (float) point.Y);
                    graphics.RotateTransform(angle);
                    graphics.DrawString(chArray[j].ToString(), font, brush, 1f, 1f, format);
                    graphics.RotateTransform(-angle);
                    graphics.TranslateTransform(2f, -((float) point.Y));
                }
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

        public static string GenerateCheckCode()
        {
            return GenerateRandomString(4, RandomStringMode.Mix);
        }

        private static string GenerateRandomString(int length, RandomStringMode mode)
        {
            string str = string.Empty;
            if (length != 0)
            {
                switch (mode)
                {
                    case RandomStringMode.LowerLetter:
                        for (int j = 0; j < length; j++)
                        {
                            str = str + lowerLetters[rand.Next(0, lowerLetters.Length)];
                        }
                        return str;

                    case RandomStringMode.UpperLetter:
                        for (int k = 0; k < length; k++)
                        {
                            str = str + upperLetters[rand.Next(0, upperLetters.Length)];
                        }
                        return str;

                    case RandomStringMode.Letter:
                        for (int m = 0; m < length; m++)
                        {
                            str = str + letters[rand.Next(0, letters.Length)];
                        }
                        return str;

                    case RandomStringMode.Digital:
                        for (int n = 0; n < length; n++)
                        {
                            str = str + digitals[rand.Next(0, digitals.Length)];
                        }
                        return str;
                }
                for (int i = 0; i < length; i++)
                {
                    str = str + mix[rand.Next(0, mix.Length)];
                }
            }
            return str;
        }

        private enum RandomStringMode
        {
            LowerLetter,
            UpperLetter,
            Letter,
            Digital,
            Mix
        }
    }
}

