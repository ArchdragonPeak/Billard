using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ServerApp;
class UdpServer
{
    static void Main(string[] args)
    {
        int serverport = 25566;
        UdpClient server = new(serverport, AddressFamily.InterNetwork);
        IPAddress multicastAdress = IPAddress.Parse("224.69.42.88");
        
        Console.WriteLine("Server running");
        IPEndPoint endpoint = new(IPAddress.Any, 0);
        server.JoinMulticastGroup(multicastAdress);
        
        while (true)
        {
            try
            {

                byte[] recivedBytes = server.Receive(ref endpoint);
                string returnData = Encoding.ASCII.GetString(recivedBytes);
                Console.WriteLine($"Recieved-Text: {returnData} | from IP: {endpoint.Address}:{endpoint.Port}");
                
                IPEndPoint newClient = new(endpoint.Address, endpoint.Port);
                server.Send(recivedBytes, recivedBytes.Length, newClient);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    private static int getAdapter()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.GetIPProperties().GatewayAddresses.Count > 0)
            {
                Console.WriteLine(ni.GetIPProperties().GetIPv4Properties().Index);
                Console.WriteLine(ni.OperationalStatus);
                return ni.GetIPProperties().GetIPv4Properties().Index;
            }
        }
        return -1;
    }
}