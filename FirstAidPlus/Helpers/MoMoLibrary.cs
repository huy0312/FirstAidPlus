using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FirstAidPlus.Helpers
{
    public class MoMoLibrary
    {
        public static string HmacSHA256(string inputData, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                return hex;

            }
        }

        public static string GetSignature(string text, string key)
        {
            return HmacSHA256(text, key);
        }
    }
}
