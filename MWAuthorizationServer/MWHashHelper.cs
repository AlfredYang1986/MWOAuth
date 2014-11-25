using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MWAuthorizationServer
{
    class MWHashHelper
    {
        public static byte[] createIdentifierKey(string seed_1, string seed_2)
        {
            using (MD5 md5 = MD5.Create())
            {
                //byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(seed_1 + seed_2));
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed_1 + seed_2));
                return new Guid(hash).ToByteArray();
            }
        }

        public static string ByteToString(byte[] b)
        {
            return Convert.ToBase64String(b.ToArray());
        }

        public static byte[] addTimeStamp(byte[] b)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            return time.Concat(b).ToArray();
        }

        public static byte[] addRandomByte(byte[] b)
        {
            byte[] b1 = new byte[0];
            var v = new Random();
            v.NextBytes(b1);
            if (b == null) 
                return b1;
            else 
                return b.Concat(b1).ToArray();
        }

        /*
         * Aid in validating the token
         * To find if the token has expired
         * Added by Ayush
         * Code from Alfred
         */
        public static DateTime getDateOfToken(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when;
        }
    }
}
