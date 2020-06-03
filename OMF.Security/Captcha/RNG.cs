using System;
using System.Security.Cryptography;

namespace OMF.Security.Captcha
{
    public static class RNG
    {
        private static byte[] randb = new byte[4];
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

        public static int Next()
        {
            RNG.rand.GetBytes(RNG.randb);
            int num = BitConverter.ToInt32(RNG.randb, 0);
            if (num < 0)
                num = -num;
            return num;
        }

        public static int Next(int max)
        {
            RNG.rand.GetBytes(RNG.randb);
            int num = BitConverter.ToInt32(RNG.randb, 0) % (max + 1);
            if (num < 0)
                num = -num;
            return num;
        }

        public static int Next(int min, int max)
        {
            return RNG.Next(max - min) + min;
        }
    }
}
