using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Common;
using System.Collections.Generic;

public class Server
{
    static string name = "Server";

    static double p = 123;
    static double g = 456;

    static double a;
    static double b = 8;

    static double s;

    static CryptMethods crypt_method;

    public static List<int> GeneratePrimesNaive(int n)
    {
        List<int> primes = new List<int>();
        primes.Add(2);
        int nextPrime = 3;
        while (primes.Count < n)
        {
            int sqrt = (int)Math.Sqrt(nextPrime);
            bool isPrime = true;
            for (int i = 0; (int)primes[i] <= sqrt; i++)
            {
                if (nextPrime % primes[i] == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            if (isPrime)
            {
                primes.Add(nextPrime);
            }
            nextPrime += 2;
        }
        return primes;
    }

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
        var primes = GeneratePrimesNaive(100);
        Random rnd = new Random();
        int r = rnd.Next(primes.Count);
        p = 23;//primes[r];
        g = 5;

        var received_request_keys = ReceiveMessage(ref socket);
        Request request = JsonConvert.DeserializeObject<Request>(received_request_keys);
        Console.WriteLine("Keys requested");
        SendMessage(ref socket, "{ \"p\": " + p + ", \"g\": " + g + " }");
        Console.WriteLine("Keys sent");
        var received_a = ReceiveMessage(ref socket);

        dynamic rec_a = JsonConvert.DeserializeObject(received_a);
        a = rec_a.a;

        double B = Math.Pow(g, b) % p;

        SendMessage(ref socket, "{ \"b\": " + B + " }");

        s = Math.Pow(a, b) % p;

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
                    crypt = new XORCrypt(Convert.ToByte(s));
                    break;
                case CryptMethods.cesar:
                    crypt = new CesarCrypt(Convert.ToByte(s));
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