using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Static Functions
    /// </summary>
    public class Funcs
    {

        /// <summary>
        /// DeepCopy
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="obj">obj</param>
        /// <returns>result</returns>
        public static T DeepCopy<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Encrypt text data, password length must be 16, 24, or 32
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="value">password</param>
        /// <returns>encrypted binary data</returns>
        public static byte[] EncryptTextData(string text, string value = "12345678901234567890123456789012")
        {
            return EncryptBinaryData(UTF8Encoding.UTF8.GetBytes(text), value);
        }

        /// <summary>
        /// Decrypt text data, password length must be 16, 24, or 32
        /// </summary>
        /// <param name="data">encrypted data</param>
        /// <param name="value">password</param>
        /// <returns>decrypted text</returns>
        public static string DecryptBinaryDataToText(byte[] data, string value = "12345678901234567890123456789012")
        {

            try
            {
                byte[] bytes = DecryptBinaryData(data, value);
                return UTF8Encoding.UTF8.GetString(bytes);
            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return "";

        }

        /// <summary>
        /// Encrypt binary data, password length must be 16, 24, or 32
        /// </summary>
        /// <param name="data">binary data</param>
        /// <param name="value">password</param>
        /// <returns>encrypted binary data</returns>
        public static byte[] EncryptBinaryData(byte[] data, string value = "12345678901234567890123456789012")
        {

#if UNITY_EDITOR

            if (value == "12345678901234567890123456789012")
            {
                Debug.LogWarning("You should not use default password " + value);
            }

#endif

            try
            {

                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = ASCIIEncoding.UTF8.GetBytes(value);
                rm.Padding = PaddingMode.PKCS7;
                rm.Mode = CipherMode.ECB;

                ICryptoTransform encryptor = rm.CreateEncryptor();
                return encryptor.TransformFinalBlock(data, 0, data.Length);

            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return new byte[] { };

        }

        /// <summary>
        /// Decrypt binary data, password length must be 16, 24, or 32
        /// </summary>
        /// <param name="data">encrypted binary data</param>
        /// <param name="value">password</param>
        /// <returns>decrypted binary data</returns>
        public static byte[] DecryptBinaryData(byte[] data, string value = "12345678901234567890123456789012")
        {

            try
            {

                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = ASCIIEncoding.UTF8.GetBytes(value);
                rm.Padding = PaddingMode.PKCS7;
                rm.Mode = CipherMode.ECB;

                ICryptoTransform decryptor = rm.CreateDecryptor();
                return decryptor.TransformFinalBlock(data, 0, data.Length);

            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return new byte[] { };

        }

        /// <summary>
        /// Copy stream
        /// </summary>
        /// <param name="src">src</param>
        /// <param name="dest">dest</param>
        /// <param name="length"read length></param>
        static void CopyTo(Stream src, Stream dest, int length = 4096)
        {
            byte[] bytes = new byte[length];

            int cnt = 0;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Zip text
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>zipped data</returns>
        public static byte[] Zip(string text)
        {
            return Zip(ASCIIEncoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Zip data
        /// </summary>
        /// <param name="bytes">data</param>
        /// <returns>zipped data</returns>
        public static byte[] Zip(byte[] bytes)
        {

            MemoryStream mso = null;

            using (MemoryStream msi = new MemoryStream(bytes))
            {
                using (mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(msi, gs);
                    }
                }
            }

            return mso.ToArray();

        }

        /// <summary>
        /// Unzip data and return text
        /// </summary>
        /// <param name="bytes">zipped data</param>
        /// <returns>unzipped text</returns>
        public static string UnzipToText(byte[] bytes)
        {
            return UTF8Encoding.UTF8.GetString(Unzip(bytes));
        }

        /// <summary>
        /// Unzip data
        /// </summary>
        /// <param name="bytes">zipped data</param>
        /// <returns>unzipped data</returns>
        public static byte[] Unzip(byte[] bytes)
        {

            MemoryStream mso = null;

            using (MemoryStream msi = new MemoryStream(bytes))
            {
                using (mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        CopyTo(gs, mso);
                    }
                }
            }

            return mso.ToArray();

        }

    }

}
