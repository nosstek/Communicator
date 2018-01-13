using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

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

            byte[] buffer = new byte[100];
            int data_received = socket.Receive(buffer);
            Console.WriteLine("Recieved...");
            for (int i = 0; i < data_received; i++)
                Console.Write(Convert.ToChar(buffer[i]));

            ASCIIEncoding encoder = new ASCIIEncoding();
            socket.Send(encoder.GetBytes("The string was recieved by the server."));
            Console.WriteLine("\nSent Acknowledgement");

            socket.Close();
            my_listener.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }
}