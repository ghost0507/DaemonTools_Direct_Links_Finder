using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace daemonTools_Direct_Links_Finder
{
    public class AESCrypter
    {
        private const int ASCII_OFFSET = 97;
        private const int BLOCK_SIZE = 16;
        private static readonly byte[] EncryptKey = AESCrypter.StringToByteArray("828d7a5c304cf43e379de0208015c928");
        private static readonly byte[] DecryptKey = AESCrypter.StringToByteArray("9c30891ec516722d206265bcf26d79cb");

        public static string EncryptString(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(message);
            int num = 16 - message.Length % 16;
            if (num == 0)
            {
                num = 16;
            }
            char value = Convert.ToChar(97 + num);
            for (int i = 0; i < num; i++)
            {
                stringBuilder.Append(value);
            }
            Random random = new Random();
            byte[] array = new byte[16];
            random.NextBytes(array);
            string result = null;
            AesManaged aesManaged = null;
            try
            {
                aesManaged = new AesManaged();
                aesManaged.Key = AESCrypter.EncryptKey;
                aesManaged.IV = array;
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.None;
                ICryptoTransform transform = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(stringBuilder.ToString());
                        }
                        byte[] array2 = memoryStream.ToArray();
                        byte[] array3 = new byte[array.Length + array2.Length];
                        Buffer.BlockCopy(array, 0, array3, 0, array.Length);
                        Buffer.BlockCopy(array2, 0, array3, array.Length, array2.Length);
                        result = Convert.ToBase64String(array3);
                    }
                }
            }
            catch (CryptographicException)
            {
                return null;
            }
            finally
            {
                if (aesManaged != null)
                {
                    aesManaged.Clear();
                }
            }
            return result;
        }

        public static string DecryptString(string cipherData)
        {
            byte[] array = Convert.FromBase64String(cipherData);
            byte[] array2 = new byte[16];
            Array.Copy(array, array2, 16);
            byte[] array3 = new byte[array.Length - 16];
            Array.Copy(array, 16, array3, 0, array.Length - 16);
            string result;
            try
            {
                AesManaged aesManaged = new AesManaged();
                aesManaged.Key = AESCrypter.DecryptKey;
                aesManaged.IV = array2;
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.None;
                using (MemoryStream memoryStream = new MemoryStream(array3))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesManaged.CreateDecryptor(AESCrypter.DecryptKey, array2), CryptoStreamMode.Read))
                    {
                        string text = new StreamReader(cryptoStream).ReadToEnd();
                        if (text.Length > 1)
                        {
                            int num = (int)(text[text.Length - 1] - 'a');
                            text = text.Substring(0, text.Length - num);
                        }
                        result = text;
                    }
                }
            }
            catch (CryptographicException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] array = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return array;
        }
    }
}
