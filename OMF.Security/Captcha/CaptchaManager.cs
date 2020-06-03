using OMF.Common;
using OMF.Common.Cache;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static OMF.Common.Enums;

namespace OMF.Security.Captcha
{
    public static class CaptchaManager
    {
        public static string GetCaptchaBase64(
          int length = 5,
          CaptchaFormat captchaFormat = CaptchaFormat.Numeric,
          int width = 120,
          int height = 50)
        {
            return Convert.ToBase64String(CaptchaManager.GetCaptchaByteArray(length, captchaFormat, width, height));
        }

        public static byte[] GetCaptchaByteArray(
          int length = 5,
          CaptchaFormat captchaFormat = CaptchaFormat.Numeric,
          int width = 120,
          int height = 50)
        {
            Image captchaImage = CaptchaManager.GetCaptchaImage(5, Enums.CaptchaFormat.Numeric, 120, 50);
            MemoryStream memoryStream = new MemoryStream();
            captchaImage.Save((Stream)memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }

        public static Image GetCaptchaImage(
          int length = 5,
          CaptchaFormat captchaFormat = CaptchaFormat.Numeric,
          int width = 120,
          int height = 50)
        {
            CaptchaImage captchaImage = new CaptchaImage(length, captchaFormat, Color.Transparent, width, height);


            SessionManager.Add("__Captcha__", (object)captchaImage);
            return (Image)captchaImage.Image;
        }

        public static bool VerifyCaptcha(string captchaValue)
        {
            CaptchaImage data = SessionManager.GetData("__Captcha__") as CaptchaImage;
            if (data == null || captchaValue == null)
                return false;
            bool flag = data.Value.ToLower() == captchaValue.ToLower();
            SessionManager.Remove("__Captcha__");
            return flag;
        }
    }
}
