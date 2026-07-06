using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class ServerScan
    {
        private readonly int _port;

        public ServerScan(int port = 6001)
        {
            _port = port;
        }

        public async Task<IPEndPoint> ScanAsync(CancellationToken token = default)
        {
            UdpClient udpClient = new UdpClient();

            var message = Encoding.UTF8.GetBytes("Hello Server");
            var endPoint = new IPEndPoint(IPAddress.Broadcast, _port);
            await udpClient.SendAsync(message, endPoint, token);

            var receive = await udpClient.ReceiveAsync(token);
            var receivedMessage = Encoding.UTF8.GetString(receive.Buffer);
            int port = -1;


            if (receivedMessage.StartsWith("I`m Serwer: "))
            {
                port = int.Parse(receivedMessage.Substring(receivedMessage.IndexOf(':') + 1).Trim());
            }

            Console.WriteLine();
            Console.WriteLine($"{receive.RemoteEndPoint.Address}:{receive.RemoteEndPoint.Port}");
            return new IPEndPoint(receive.RemoteEndPoint.Address, port);
        }
    }
}
