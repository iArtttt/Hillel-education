using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class UdpServer
    {
        private readonly int _port;

        public UdpServer(int port)
        {
            _port = port;
        }

        public async Task Listen(CancellationToken token = default)
        {
            var udp = new UdpClient(_port);

            do
            {
                var receivedData = await udp.ReceiveAsync(token);
                var message = Encoding.UTF8.GetString(receivedData.Buffer);

                Console.WriteLine(message);

                if (message == "Hello Server")
                {
                    message = $"I`m Serwer: {Server.Port}";
                    await udp.SendAsync(Encoding.UTF8.GetBytes(message), receivedData.RemoteEndPoint, token);
                }
            } while (!token.IsCancellationRequested);
        }
    }
}
