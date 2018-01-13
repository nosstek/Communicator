using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using Common;

public class Client
{
    static String name = "Client";

    static int p;
    static int g;

    static int a;
    static int b;

    static String SendMessage(ref TcpClient tcp_client, String message)
    {
        String write_message = JsonConvert.SerializeObject(message);
        Stream network_stream = tcp_client.GetStream();

        ASCIIEncoding encoder = new ASCIIEncoding();
        byte[] write_buffer = encoder.GetBytes(write_message);
        Console.WriteLine("Transmitting.....");

        network_stream.Write(write_buffer, 0, write_buffer.Length);

        byte[] read_buffer = new byte[100];
        int data_received = network_stream.Read(read_buffer, 0, 100);

        return encoder.GetString(read_buffer, 0, data_received); ;
    }

    static bool InitializeSecureConnection(ref TcpClient tcp_client)
    {
        return true;

        Request req = new Request("keys");
        String request_keys = JsonConvert.SerializeObject(req);
        String request_keys_response = SendMessage(ref tcp_client, request_keys);

        dynamic response_p_i_g = JsonConvert.DeserializeObject(request_keys_response);
        p = response_p_i_g.p;
        g = response_p_i_g.g;

        String send_a_response = SendMessage(ref tcp_client, "{ \"a\": 123 }");

        dynamic response_b = JsonConvert.DeserializeObject(send_a_response);
        b = response_b.b;

        Encryption encryption = new Encryption("none");
        String enc = JsonConvert.SerializeObject(encryption);
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

            bool quit = false;
            do
            {
                Console.Write("Enter the string to be transmitted : ");

                String from = Client.name;
                String msg = Console.ReadLine();
                Message message = new Message(from, msg);
                String write_message = JsonConvert.SerializeObject(message);
                Stream network_stream = tcp_client.GetStream();

                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] write_buffer = encoder.GetBytes(write_message);
                Console.WriteLine("Transmitting.....");

                network_stream.Write(write_buffer, 0, write_buffer.Length);

                byte[] read_buffer = new byte[100];
                int data_received = network_stream.Read(read_buffer, 0, 100);

                String read_message = encoder.GetString(read_buffer, 0, data_received);
                Console.WriteLine(read_message);
                //Console.WriteLine(SendMessage(tcp_client, JsonConvert.SerializeObject(message)));

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