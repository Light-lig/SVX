using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SVX.Models
{
    public class Util
    {
        public string GenerarCodigo(String tipo)
        {
            string random_bytes = null;
            string sal = null;
            byte[] Brandom = new Byte[200];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(Brandom);
            random_bytes = System.Text.RegularExpressions.Regex.Replace(Encoding.UTF8.GetString(Brandom), "[^0-9a-zA-Z]+", "");
            sal = Base64Encode(random_bytes);
            sal = sal.Replace('+', ' ').Trim();
            sal = sal.Replace('=', ' ').Trim();
            sal = sal.Substring(0, 24);
            string a = sal.Substring(0, 12);
            string b = sal.Substring(12, 12);
            string result = a + System.Guid.NewGuid().ToString("N") + b + tipo;
            return result;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}