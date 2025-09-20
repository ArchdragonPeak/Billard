using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameCore;

namespace BilliardEnv
{
    public class Server
    {
        private TcpListener listener;
        private bool running = false;

        private Game game;

        public Server(int port = 5555)
        {
            listener = new TcpListener(IPAddress.Any, port);
            game = new Game();
        }

        public void Start()
        {
            listener.Start();
            running = true;
            Console.WriteLine("Server listening...");

            while (running)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                new Thread(() => HandleClient(client)).Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];

            while (client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                string[] parts = msg.Split(' ');

                if (parts[0] == "reset")
                {
                    game.RandomReset();
                    SendObservation(stream);
                }
                else if (parts[0] == "step")
                {
                    float theta = float.Parse(parts[1]);
                    int strength = (int)float.Parse(parts[2]);

                    game.Shoot(theta, strength);
                    while (game.ballsMoving)
                    {
                        game.Update();
                        //Debug.WriteLine("updating");
                    }

                    //Debug.WriteLine("sending observation");
                    SendObservation(stream);
                }
                else if (parts[0] == "close")
                {
                    client.Close();
                }
            }
        }

        private void SendObservation(NetworkStream stream)
        {
            var table = game.GetTable();

            var sb = new StringBuilder();
            foreach (var (x, y) in table)
            {
                sb.Append(x).Append(' ').Append(y).Append(' ');
            }

            byte[] data = Encoding.UTF8.GetBytes(sb.ToString().Trim());
            stream.Write(data, 0, data.Length);
        }
    }
}
