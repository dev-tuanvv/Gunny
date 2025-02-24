namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class ValidateCode : Page
    {
        protected Button Button1;
        public static Color[] colors = new Color[] { Color.Blue, Color.DarkRed, Color.Green, Color.Gold };
        protected HtmlForm form1;

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.CreateCheckCodeImage(this.GenerateCheckCode());
        }

        private void CreateCheckCodeImage(string checkCode)
        {
            if ((checkCode != null) && (checkCode.Trim() != string.Empty))
            {
                Bitmap image = new Bitmap((int) Math.Ceiling((double) (checkCode.Length * 40.5)), 0x2c);
                Graphics graphics = Graphics.FromImage(image);
                try
                {
                    Random random = new Random();
                    Color color = colors[random.Next(colors.Length)];
                    graphics.Clear(Color.Transparent);
                    for (int i = 0; i < 2; i++)
                    {
                        int num8 = random.Next(image.Width);
                        int num9 = random.Next(image.Width);
                        int num10 = random.Next(image.Height);
                        int num11 = random.Next(image.Height);
                        graphics.DrawArc(new Pen(color, 2f), -num8, -num10, image.Width * 2, image.Height, 0x2d, 100);
                    }
                    Font font = new Font("Arial", 24f, FontStyle.Italic | FontStyle.Bold);
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), color, color, 1.2f, true);
                    graphics.DrawString(checkCode, font, brush, (float) 2f, (float) 2f);
                    int num = 40;
                    double num2 = Math.Sin((3.1415926535897931 * num) / 180.0);
                    double num3 = Math.Cos((3.1415926535897931 * num) / 180.0);
                    double num4 = Math.Atan((3.1415926535897931 * num) / 180.0);
                    int num5 = 0;
                    int num6 = 0;
                    if (num > 0)
                    {
                        num5 = (int) (num2 * 20.0);
                        num6 = (int) (-num2 * image.Width);
                    }
                    else
                    {
                        num6 = (int) (-num2 * 22.0);
                    }
                    new TextureBrush(image).RotateTransform(30f);
                    image.Save(@"c:\1.jpg", ImageFormat.Png);
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Png);
                    base.Response.ClearContent();
                    base.Response.ContentType = "image/Gif";
                    base.Response.BinaryWrite(stream.ToArray());
                }
                finally
                {
                    graphics.Dispose();
                    image.Dispose();
                }
            }
        }

        private string GenerateCheckCode()
        {
            string str = string.Empty;
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                int num = random.Next();
                str = str + ((char) (0x41 + ((ushort) (num % 0x1a)))).ToString();
            }
            return str;
        }

        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            PixelFormat pixelFormat;
            int width = bmp.Width + 2;
            int height = bmp.Height + 2;
            if (bkColor == Color.Transparent)
            {
                pixelFormat = PixelFormat.Format32bppArgb;
            }
            else
            {
                pixelFormat = bmp.PixelFormat;
            }
            Bitmap image = new Bitmap(width, height, pixelFormat);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(bkColor);
            graphics.DrawImageUnscaled(bmp, 1, 1);
            graphics.Dispose();
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, (float) width, (float) height));
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            RectangleF bounds = path.GetBounds(matrix);
            Bitmap bitmap2 = new Bitmap((int) bounds.Width, (int) bounds.Height, pixelFormat);
            graphics = Graphics.FromImage(bitmap2);
            graphics.Clear(bkColor);
            graphics.TranslateTransform(-bounds.X, -bounds.Y);
            graphics.RotateTransform(angle);
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.DrawImageUnscaled(image, 0, 0);
            graphics.Dispose();
            image.Dispose();
            return bitmap2;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            byte[] buffer = CheckCode.CreateImage(CheckCode.GenerateCheckCode());
            base.Response.ClearContent();
            base.Response.ContentType = "image/Gif";
            base.Response.BinaryWrite(buffer);
        }
    }
}

