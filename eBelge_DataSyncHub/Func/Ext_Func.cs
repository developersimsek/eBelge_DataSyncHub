#region Using

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace eBelge_DataSyncHub.Func
{
    #region String

    public static class StringExtensions
    {
        /// <summary>
        /// String ifadedeki birden fazla boşlukları tek boşluğa dönüştürür.
        /// </summary>
        /// <param name="input">İşlem yapılacak String ifade.</param>
        /// <returns>Boşlukların tek boşluğa düşürülmüş hali.</returns>
        public static string RemoveExtraSpaces(this string input)
        {
            string pattern = "\\s+";
            string result = Regex.Replace(input, pattern, " ");
            return result;
        }

        /// <summary>
        /// String ifadedeki noktalama işaretlerinden sonraki harfleri büyük, diğerleri küçük harfe dönüştürür.
        /// </summary>
        /// <param name="input">İşlem yapılacak String ifade.</param>
        /// <returns>Büyük/Küçük işlemlerinin yapılmış Strinf ifade.</returns>
        /// <returns>Büyük/Küçük işlemlerinin yapılmış Strinf ifade.</returns>
        public static string CapitalizeSentences(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            bool capitalize = true;

            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsLetter(chars[i]))
                {
                    chars[i] = capitalize ? char.ToUpper(chars[i]) : char.ToLower(chars[i]);
                    capitalize = false;
                }
                else if (chars[i] == '.' || chars[i] == '!' || chars[i] == '?')
                {
                    capitalize = true;
                }
            }

            return new string(chars);
        }

        /// <summary>
        /// Verilen string tarih formatını "dd.MM.yyyy" biçiminden "yyyy-MM-dd" biçimine dönüştürür.
        /// </summary>
        /// <param name="inputDate">Dönüştürülecek olan tarih string'i</param>
        /// <returns>Dönüştürülmüş tarih string'i veya orijinal giriş string'i (dönüşüm başarısız olduğunda)</returns>
        public static string ConvertDateFormat(this string inputDate)
        {

            if (DateTime.TryParseExact(inputDate, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }
            if (DateTime.TryParseExact(inputDate, "dd MMMM yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }

            return inputDate;
        }

        public class Ext_String
        {
            public static string GenerateStringKey(int length = 20)
            {
                try
                {
                    byte[] data = new byte[length];
                    using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(data);
                    }
                    StringBuilder result = new StringBuilder(length);
                    foreach (byte b in data)
                    {
                        result.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[b % ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length)]);
                    }
                    return result.ToString();
                }
                catch (Exception ex)
                {

                    throw new Exception("GenerateStringKey İşlemler sırasında bir hata oluştu." + ex.Message);
                }
            }

        }

    }

    #endregion
}