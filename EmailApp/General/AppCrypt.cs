using System.Text;
using System.Security.Cryptography;

namespace MailAppMAUI.General
{
    /// <summary> Encripta o desecripta claves de la aplicacion
    /// </summary>

    public class AppCrypt
    {
        private static byte[] CodKey = Encoding.ASCII.GetBytes("KS678UxPt403rWxz");
        private static byte[] CodIV = Encoding.ASCII.GetBytes("Dtx1245.ClrA4672");

        public static string TryDecode(string cadena)
        {
            try
            {
                cadena = Decode(cadena);
            }
            catch (Exception exc)
            {
                cadena = null;
                // Logger.LogError(exc);
            }
            return cadena;
        }

        public static string Encode(string Cadena)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(Cadena);
            byte[] encripted;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms,
                       cripto.CreateEncryptor(CodKey, CodIV),
                       CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }
            return Convert.ToBase64String(encripted);
        }


        public static string Decode(string Cadena)
        {
            byte[] inputBytes = Convert.FromBase64String(Cadena);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = string.Empty;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms,
                       cripto.CreateDecryptor(CodKey, CodIV),
                       CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(objCryptoStream, true))
                    {
                        textoLimpio = sr.ReadToEnd();
                    }
                }
            }
            return textoLimpio;
        }

        public static string FromBase64(string Cadena)
        {
            byte[] bytes = Convert.FromBase64String(Cadena);
            string result = Encoding.ASCII.GetString(bytes);

            return result;
        }
    }

}
