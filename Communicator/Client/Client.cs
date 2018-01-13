using System;
using System.IO;
using System.Text;
using System.Net.Sockets;


public class Client
{
    public static void Main(string[] args)
    {
        try
        {
            TcpClient tcp_client = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcp_client.Connect("192.168.0.229", 8001);

            Console.WriteLine("Connected");
            Console.Write("Enter the string to be transmitted : ");

            String message = Console.ReadLine();
            Stream network_stream = tcp_client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] write_buffer = encoder.GetBytes(message);
            Console.WriteLine("Transmitting.....");

            network_stream.Write(write_buffer, 0, write_buffer.Length);

            byte[] read_buffer = new byte[100];
            int data_received = network_stream.Read(read_buffer, 0, 100);

            for (int i = 0; i < data_received; i++)
                Console.Write(Convert.ToChar(read_buffer[i]));

            tcp_client.Close();
        }

        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }
}