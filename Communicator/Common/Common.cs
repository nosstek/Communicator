using System;

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
        public Encryption(String enc)
        {
            encryption = enc;
        }
        public String encryption;
    }

    public interface IEncoder
    {
        string Encode(string plainText);
        string Decode(string encodedData);
    }

    public class Base64Encoder : IEncoder
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

    public class AsciiEncoder : IEncoder
    {
        public string Decode(string encodedData)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
        }

        public string Encode(string plainText)
        {
            throw new NotImplementedException();
        }
    }

}
