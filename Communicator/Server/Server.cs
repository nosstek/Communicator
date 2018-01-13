using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Common;

public class Server
{
    static string name = "Server";

    static int p = 123;
    static int g = 456;

    static int a;
    static int b = 8;

    static CryptMethods crypt_method;

    static string ReceiveMessage(ref Socket socket)
    {
        byte[] buffer = new byte[100];
        int data_received = socket.Receive(buffer);
        string json_message = Encoding.UTF8.GetString(buffer, 0, data_received);
        Console.WriteLine("Recieved..." + json_message);
        return json_message;
    }

    static void SendMessage(ref Socket socket, string message)
    {
        socket.Send(Encoding.UTF8.GetBytes(message));
    }

    static bool InitializeSecureConnection(ref Socket socket)
    {
        var received_request_keys = ReceiveMessage(ref socket);
        Request request = JsonConvert.DeserializeObject<Request>(received_request_keys);
        Console.WriteLine("Keys requested");
        SendMessage(ref socket, "{ \"p\": " + p + ", \"g\": " + g + " }");
        Console.WriteLine("Keys sent");
        var received_a = ReceiveMessage(ref socket);

        dynamic rec_a = JsonConvert.DeserializeObject(received_a);
        a = rec_a.a;

        SendMessage(ref socket, "{ \"b\": " + b + " }");

        var received_encryption = ReceiveMessage(ref socket);
        dynamic encryption_method = JsonConvert.DeserializeObject(received_encryption);
        crypt_method = encryption_method.encryption;

        SendMessage(ref socket, "Connection Succesfull");

        return true;
    }

        public static void Main(string[] args)
    {
        if (args[0] == null)
        {
            Console.WriteLine("Give server IP as a argument!");
            return;
        }

        string ip = args[0];
        try
        {
            IPAddress ip_address = IPAddress.Parse(ip);

            TcpListener my_listener = new TcpListener(ip_address, 8001);

            my_listener.Start();

            Console.WriteLine("The server is running at port 8001...");
            Console.WriteLine("The local End point is  :" + my_listener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");

            Socket socket = my_listener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + socket.RemoteEndPoint);

            InitializeSecureConnection(ref socket);

            ICrypt crypt;
            
            switch (crypt_method)
            {
                case CryptMethods.xor:
                    crypt = new XORCrypt();
                    break;
                case CryptMethods.cesar:
                    crypt = new CesarCrypt();
                    break;
                case CryptMethods.none:
                default:
                    crypt = new NoCrypt();
                    break;
            };

            ICoder coder = new Base64Coder();

            bool quit = false;
            do
            {
                byte[] buffer = new byte[100];
                int data_received = socket.Receive(buffer);
                Console.Write("Recieved... ");

                string json_message = Encoding.UTF8.GetString(buffer, 0, data_received);
                Console.WriteLine(json_message);

                Message message = JsonConvert.DeserializeObject<Message>(json_message);
                string name = message.Name;
                string text_encoded = message.Text;
                string text_encrypted = coder.Decode(text_encoded);
                string text = crypt.Decrypt(text_encrypted);
                Console.WriteLine(name + ": " + text);

                socket.Send(Encoding.UTF8.GetBytes("The string was recieved by the server."));
                Console.WriteLine("Sent Acknowledgement");

                quit = text == "quit";
            } while (!quit);
            socket.Close();
            my_listener.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
        Console.ReadKey();
    }
}