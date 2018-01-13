using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Common;
using System.Text.RegularExpressions;

public class Server
{
    static String name = "Server";

    static int p;
    static int g;

    static int a;
    static int b;


    static String ReceiveMessage(ref Socket socket)
    {
        byte[] buffer = new byte[100];
        int data_received = socket.Receive(buffer);
        ASCIIEncoding encoder = new ASCIIEncoding();
        String json_message = encoder.GetString(buffer, 0, data_received);
        Console.WriteLine("Recieved..." + json_message);
        return Regex.Unescape(json_message);
    }

    static void SendMessage(ref Socket socket, String message)
    {
        ASCIIEncoding encoder = new ASCIIEncoding();
        socket.Send(encoder.GetBytes(message));
    }

    static bool InitializeSecureConnection(ref Socket socket)
    {
        return true;

        var received_request_keys = ReceiveMessage(ref socket);
        Request request = JsonConvert.DeserializeObject<Request>(received_request_keys);
        Console.WriteLine("Keys requested");
        SendMessage(ref socket, "{ \"p\": 123, \"g\": 123 }");
        Console.WriteLine("Keys sent");
        var received_a = ReceiveMessage(ref socket);

        dynamic rec_a = JsonConvert.DeserializeObject(received_a);
        //a = rec_a.a;

        SendMessage(ref socket, "{ \"b\": 123 }");

        var received_encryption = ReceiveMessage(ref socket);

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

            bool quit = false;
            do
            {
                byte[] buffer = new byte[100];
                int data_received = socket.Receive(buffer);
                Console.Write("Recieved... ");

                ASCIIEncoding encoder = new ASCIIEncoding();
                String json_message = encoder.GetString(buffer, 0, data_received);
                Console.Write(json_message);

                Message message = JsonConvert.DeserializeObject<Message>(json_message);
                string name = message.Name;
                string text = message.Text;
                Console.WriteLine(name + ": " + text);

                socket.Send(encoder.GetBytes("The string was recieved by the server."));
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