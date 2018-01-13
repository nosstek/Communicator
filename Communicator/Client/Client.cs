using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

public class Client
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
            TcpClient tcp_client = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcp_client.Connect(ip, 8001);

            Console.WriteLine("Connected");


            bool quit = false;
            do
            {
                Console.Write("Enter the string to be transmitted : ");

                String write_message = Console.ReadLine();
                Stream network_stream = tcp_client.GetStream();
                
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] write_buffer = encoder.GetBytes(write_message);
                Console.WriteLine("Transmitting.....");

                network_stream.Write(write_buffer, 0, write_buffer.Length);

                byte[] read_buffer = new byte[100];
                int data_received = network_stream.Read(read_buffer, 0, 100);

                String read_message = encoder.GetString(read_buffer, 0, data_received);
                Console.WriteLine(read_message);

                quit = write_message == "quit";
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