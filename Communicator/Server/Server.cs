using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

public class Server
{
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
            bool quit = false;
            do
            {
                byte[] buffer = new byte[100];
                int data_received = socket.Receive(buffer);
                Console.WriteLine("Recieved...");

                ASCIIEncoding encoder = new ASCIIEncoding();
                String json_message = encoder.GetString(buffer, 0, data_received);
                dynamic message = JsonConvert.DeserializeObject(json_message);
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