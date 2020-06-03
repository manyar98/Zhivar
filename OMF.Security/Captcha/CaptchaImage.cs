using OMF.Common;
using OMF.Common.Extensions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using static OMF.Common.Enums;

namespace OMF.Security.Captcha
{
    public class CaptchaImage : IDisposable
    {
        private int minDistort = 5;
        private int maxDistort = 10;
        private FontFamily[] fonts = new FontFamily[4]
        {
      new FontFamily("Times New Roman"),
      new FontFamily("Georgia"),
      new FontFamily("Arial"),
      new FontFamily("Comic Sans MS")
        };
        public const string Alphabetic = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Numeric = "0123456789";
        public const string AlphaNumeric = "AB1CDE5FG4HIJ0KL6MN3OPQR2STUV8WX7Y9Z";
        private CaptchaFormat captchaFormat;

        public string Value { get; private set; }

        public Color BackgroundColor { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Bitmap Image { get; private set; }

        public int CaptchaLength { get; private set; }

        public Enums.CaptchaFormat CaptchaFormat
        {
            get
            {
                return this.captchaFormat;
            }
            set
            {
                if (this.captchaFormat == value)
                    return;
                this.captchaFormat = value;
                int millisecond = DateTime.Now.Millisecond;
                DateTime now = DateTime.Now;
                int second = now.Second;
                now = DateTime.Now;
                int num1 = now.Minute * 60;
                int num2 = (second + num1) * 1000;
                int Seed = millisecond + num2;
                switch (value)
                {
                    case Enums.CaptchaFormat.Alphabetic:
                        for (int index = 0; index < this.CaptchaLength; ++index)
                        {
                            Seed += 10001;
                            this.Value += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[new Random(Seed).Next(0, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Length - 1)].ToString();
                        }
                        break;
                    case Enums.CaptchaFormat.Numeric:
                        this.minDistort = 7;
                        this.maxDistort = 15;
                        int minValue = string.Join<int>("", Enumerable.Repeat<int>(1, this.CaptchaLength)).ConvertTo<int>();
                        int maxValue = string.Join<int>("", Enumerable.Repeat<int>(9, this.CaptchaLength)).ConvertTo<int>();
                        this.Value = new Random(Seed).Next(minValue, maxValue).ToString();
                        break;
                    case Enums.CaptchaFormat.AlphaNumeric:
                        for (int index = 0; index < this.CaptchaLength; ++index)
                        {
                            Seed += 10001;
                            this.Value += "AB1CDE5FG4HIJ0KL6MN3OPQR2STUV8WX7Y9Z"[new Random(Seed).Next(0, "AB1CDE5FG4HIJ0KL6MN3OPQR2STUV8WX7Y9Z".Length - 1)].ToString();
                        }
                        break;
                }
            }
        }

        public CaptchaImage()
          : this(5)
        {
        }

        public CaptchaImage(int length)
          : this(length, Enums.CaptchaFormat.Numeric)
        {
        }

        public CaptchaImage(int length, Enums.CaptchaFormat captchaFormat)
          : this(length, captchaFormat, Color.Transparent, 150, 50)
        {
        }

        public CaptchaImage(
          int length,
          Enums.CaptchaFormat captchaFormat,
          Color backgroundColor,
          int width,
          int height)
        {
            this.CaptchaLength = length > 5 ? length : 5;
            this.CaptchaFormat = captchaFormat;
            this.BackgroundColor = backgroundColor;
            this.Width = width < 100 ? 100 : width;
            this.Height = height < 50 ? 50 : height;
            this.GenerateImage();
        }

        public void Dispose()
        {
            GC.SuppressFinalize((object)this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.Image.Dispose();
        }

        private void GenerateImage()
        {
            Bitmap bitmap1 = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap1);
            Rectangle rectangle = new Rectangle(0, 0, this.Width, this.Height);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush solidBrush = new SolidBrush(this.BackgroundColor))
                graphics.FillRectangle((Brush)solidBrush, rectangle);
            Font font = new Font(this.fonts[RNG.Next(this.fonts.Length - 1)], 32f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            GraphicsPath path = new GraphicsPath();
            path.AddString(this.Value, font.FontFamily, (int)font.Style, font.Size, rectangle, format);
            int int32 = Convert.ToInt32(this.BackgroundColor.R);
            int red = RNG.Next((int)byte.MaxValue);
            int green = RNG.Next((int)byte.MaxValue);
            int blue = RNG.Next((int)byte.MaxValue);
            while (red >= int32 && red - 20 <= int32 || red < int32 && red + 20 >= int32)
                red = RNG.Next(0, (int)byte.MaxValue);
            SolidBrush solidBrush1 = new SolidBrush(Color.FromArgb(red, green, blue));
            graphics.FillPath((Brush)solidBrush1, path);
            Pen pen = new Pen(Color.FromArgb(red, green, blue), 0.8f);
            graphics.DrawLine(pen, this.Width / 4, this.Height / 2, this.Width - this.Width / 4, this.Height / 2);
            double num = (double)(RNG.Next(this.minDistort, this.maxDistort) * (RNG.Next(100) % 2 == 1 ? 1 : -1));
            using (Bitmap bitmap2 = (Bitmap)bitmap1.Clone())
            {
                for (int y1 = 0; y1 < this.Height; ++y1)
                {
                    for (int x1 = 0; x1 < this.Width; ++x1)
                    {
                        int x2 = (int)((double)x1 + num * Math.Sin(Math.PI * (double)y1 / 84.0));
                        int y2 = (int)((double)y1 + num * Math.Cos(Math.PI * (double)x1 / 44.0));
                        if (x2 < 0 || x2 >= this.Width)
                            x2 = 0;
                        if (y2 < 0 || y2 >= this.Height)
                            y2 = 0;
                        bitmap1.SetPixel(x1, y1, bitmap2.GetPixel(x2, y2));
                    }
                }
            }
            font.Dispose();
            solidBrush1.Dispose();
            graphics.Dispose();
            this.Image = bitmap1;
        }
    }
}
