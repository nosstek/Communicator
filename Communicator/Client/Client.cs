using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using Common;

public class Client
{
    static string name = "Client";

    static double p;
    static double g;

    static double a = 4;
    static double b;

    static double s;

    static CryptMethods crypt_method = CryptMethods.cesar;

    static string SendMessage(ref TcpClient tcp_client, string message)
    {
        Stream network_stream = tcp_client.GetStream();

        byte[] write_buffer = Encoding.UTF8.GetBytes(message);
        Console.WriteLine("Transmitting.....");

        network_stream.Write(write_buffer, 0, write_buffer.Length);

        byte[] read_buffer = new byte[100];
        int data_received = network_stream.Read(read_buffer, 0, 100);

        return Encoding.UTF8.GetString(read_buffer, 0, data_received);
    }

    static bool InitializeSecureConnection(ref TcpClient tcp_client)
    {
        Request req = new Request("keys");
        string request_keys = JsonConvert.SerializeObject(req);
        string request_keys_response = SendMessage(ref tcp_client, request_keys);

        dynamic response_p_i_g = JsonConvert.DeserializeObject(request_keys_response);
        p = response_p_i_g.p;
        g = response_p_i_g.g;

        double A = Math.Pow(g, a) % p;

        string a_string = "{ \"a\": " + A + " }";
        string send_a_response = SendMessage(ref tcp_client, a_string);

        dynamic response_b = JsonConvert.DeserializeObject(send_a_response);
        b = response_b.b;

        s = Math.Pow(b, a) % p;

        Encryption encryption = new Encryption(crypt_method);
        string enc = JsonConvert.SerializeObject(encryption);
        Console.WriteLine(SendMessage(ref tcp_client, enc));

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
            TcpClient tcp_client = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcp_client.Connect(ip, 8001);

            Console.WriteLine("Connected");

            InitializeSecureConnection(ref tcp_client);

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
                Console.Write("Enter the string to be transmitted : ");

                string from = Client.name;

                string msg = Console.ReadLine();
                string encrypted_msg = crypt.Encrypt(msg);
                string encoded_msg = coder.Encode(encrypted_msg);

                Message message = new Message(from, encoded_msg);

                string write_message = JsonConvert.SerializeObject(message);

                Console.WriteLine(SendMessage(ref tcp_client, write_message));

                quit = msg == "quit";
            } while (!quit);

            tcp_client.Close();
        }
        catch (Exception e)
        {
             Console.WriteLine("Error..... " + e.StackTrace);
        }
        Console.ReadKey();
    }


}