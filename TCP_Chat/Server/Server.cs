using Server.Lib;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal static class Server
    {
        internal static int Port { get; private set; }
        private static Dictionary<string, ServerClient> clientsDictionary = new Dictionary<string, ServerClient>();
        private static List<ServerClient> clients = new List<ServerClient>();
        private static Dictionary<ClientMessageType, List<string>> story = new Dictionary<ClientMessageType, List<string>>();
        public static async Task Start(int port = 5002) 
        {
            Port = port > 1000 && port < 65535 ? port : 5002;
            var listener = new TcpListener(IPAddress.Any, Port);

            var udpServer = new UdpServer(6001);

            listener.Start();
            Console.WriteLine($"Server started on: {listener.LocalEndpoint}");
            udpServer.Listen();


            foreach (var type in Enum.GetValues(typeof(ClientMessageType)))
            {
                story.Add((ClientMessageType)type, new List<string>());
            }

            try
            {
                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    ServerClient? client = await Enterence(tcpClient);
                    GetStory(client);
                    client.Start();
                }
            }
            finally
            {
                listener.Stop();
            }
        }
        private static Task<ServerClient?> Enterence(TcpClient tcpClient)
        {
            return Task.Run(async () =>
            {

                bool isEntered = false;
                var stream = tcpClient.GetStream();
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);
                ServerClient client = new ServerClient();

                do
                {
                    ServerClient.SendMessage(writer, "Write login || Or \\+ if You are new user", SystemMessageType.System);
                
                    var spanLine = (await reader.ReadLineAsync()).AsSpan().Slice(2).ToString();
                    if(spanLine.Length == 0)
                    {
                        ServerClient.SendMessage(writer, "You can`t have empty Nick Name", SystemMessageType.System);
                    }
                    else if(spanLine == "+")
                    {
                        client = new ServerClient(tcpClient);
                        clientsDictionary.Add(await client.RegisterNickName(), client);
                        await client.SetPassword();

                        client.MessageReceive += Client_MessageReceived;
                        clients.Add(client);
                        isEntered = true;
                    }
                    else
                    {
                        client = GetClient(spanLine)!;
                        if (client == null) ServerClient.SendMessage(writer, "Client with this login does not found", SystemMessageType.System);
                        else
                        {
                            client.ReSetTcp(tcpClient, stream, reader, writer);
                            isEntered = await client.ConfirmPassword();
                        }
                    }

                } while (!isEntered);
                return client;
            });
        }
        internal static void AddStory(ClientMessageType messageType, string message) => story[messageType].Add(message);
        private static void GetStory(ServerClient? client)
        {
            if (client == null) return;
            foreach(var message in story[ClientMessageType.Message])
            {
                client.SendMessage(message, SystemMessageType.Unknown);
            }
        }
        internal static ServerClient? GetClient(string clientName) => clientsDictionary.GetValueOrDefault(clientName);
        private static void Client_MessageReceived(string? messege)
        {
            if (messege != null)
                clients.ForEach(client => client.SendMessage(messege, SystemMessageType.Message));
        }
    }
}
