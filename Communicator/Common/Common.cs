using System;
using System.Collections.Generic;

namespace Common
{
    public class Message
    {
        public Message(String name, String text)
        {
            Name = name;
            Text = text;
        }
        public String Name;
        public String Text;
    }

    public class Request
    {
        public Request(String request)
        {
            Req = request;
        }
        public String Req;
    }

    public class Encryption
    {
        public Encryption(CryptMethods enc)
        {
            encryption = enc;
        }
        public CryptMethods encryption;
    }

    public interface ICoder
    {
        string Encode(string plainText);
        string Decode(string encodedData);
    }

    public class Base64Coder : ICoder
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string Encode(string plainText)
        {
            return Base64Encode(plainText);
        }

        public string Decode(string encodedData)
        {
            return Base64Decode(encodedData);
        }
    }

    public enum CryptMethods { none, xor, cesar };

    public interface ICrypt
    {
        string Encrypt(string plainText);
        string Decrypt(string encodeData);
    }

    public class NoCrypt : ICrypt
    {
        public string Decrypt(string encodeData)
        {
            return encodeData;
        }

        public string Encrypt(string plainText)
        {
            return plainText;
        }
    }

    public class XORCrypt : ICrypt
    {
        byte key;

        public XORCrypt(byte k)
        {
            key = k;
        }

        public string Decrypt(string encodeData)
        {
            return Encrypt(encodeData);
        }

        public string Encrypt(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            List<byte> EncryptedBytes = new List<byte>();
            for (int i = 0; i < plainTextBytes.Length; ++i)
            {
                byte b = (byte)(plainTextBytes[i] ^ key);
                EncryptedBytes.Add(b);
            }

            return System.Text.Encoding.UTF8.GetString(EncryptedBytes.ToArray());
        }
    }

    public class CesarCrypt : ICrypt
    {
        byte key;

        public CesarCrypt(byte k)
        {
            key = k;
        }
        public string Decrypt(string encodeData)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(encodeData);

            List<byte> EncryptedBytes = new List<byte>();
            for (int i = 0; i < plainTextBytes.Length; ++i)
            {
                byte b = (byte)(plainTextBytes[i] - key);
                EncryptedBytes.Add(b);
            }

            return System.Text.Encoding.UTF8.GetString(EncryptedBytes.ToArray());
        }

        public string Encrypt(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            List<byte> EncryptedBytes = new List<byte>();
            for (int i = 0; i < plainTextBytes.Length; ++i)
            {
                byte b = (byte)(plainTextBytes[i] + key);
                EncryptedBytes.Add(b);
            }

            return System.Text.Encoding.UTF8.GetString(EncryptedBytes.ToArray());
        }
    }
}