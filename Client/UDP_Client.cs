using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ClientApp;
class Program
{
    static void Main(string[] args)
    {
        int serverPort = 25566;
        IPAddress multicastAdress = IPAddress.Parse("224.69.42.88");
        IPAddress? serverAdress;
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            var ipv4 = item.GetIPProperties().UnicastAddresses.FirstOrDefault(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)?.Address;

            Console.WriteLine($"IPv4: {ipv4,-15} Name: {item.Name,-20} ");
        }
        Console.WriteLine("Client running");
        serverAdress = FindServerAdress(multicastAdress, serverPort);
        Console.WriteLine(serverAdress);
    }

    private static IPAddress? FindServerAdress(IPAddress multicastAdress, int serverPort)
    {
        try
        {
            UdpClient client = new();
            IPEndPoint endPoint = new(IPAddress.Parse("192.168.178.50"), serverPort);
            byte[] senddata = Encoding.ASCII.GetBytes("Client");
            client.Send(senddata, senddata.Length, endPoint);

            endPoint = new(IPAddress.Any, 0);
            byte[] recivedBytes = client.Receive(ref endPoint);
            return endPoint.Address;
        }
        catch (Exception)
        {
            return null;
        }
    }
}