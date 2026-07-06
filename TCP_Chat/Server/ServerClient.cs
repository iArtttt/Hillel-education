using Server.Lib;
using System.Net.Sockets;

namespace Server
{
    class ServerClient : IDisposable
    {
        public string? NickName { get; private set; } = string.Empty;
        private string Password { get; set; } = "1234";
        private bool _endChating;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Task? _process;

        public event Action<string?>? MessageReceive;
        public ServerClient()
        {
            
        }
        public ServerClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
        }
        
        

        public Task Start()
        {
            return _process = Task.Run(async() => 
            {
                Log("Connected", SystemMessageType.System);
                MessageReceive?.Invoke($"[{NickName}]: Connected to the chat");
                string? line = string.Empty;
                _endChating = false;
                do
                {
                    line = ReceivedMessage(await _reader.ReadLineAsync());

                } while (!_endChating);
                Log("Disconnected", SystemMessageType.System);
                SendMessage("You left us (((", SystemMessageType.Unknown);
                await _writer.FlushAsync();
            });
        }
        private void PrivateRoom(ServerClient client)
        {
            Task.Run(async () => 
            {
                string? line = string.Empty;
                bool isExit = false;
                SendMessage("You are in Private chat now", SystemMessageType.Private);
                do
                {
                    line = await _reader.ReadLineAsync();
                    var spanLine = line.AsSpan();
                    var message = spanLine.Slice(2);
                    if (message.Length == 0)
                    {
                        client.SendMessage($"--Exit({NickName}) Left Private Chat", SystemMessageType.Private);
                        SendMessage("You Left Private chat", SystemMessageType.Private);
                        isExit = true;
                    }
                    else
                        client.SendMessage(message.ToString(), SystemMessageType.Private);

                }while (!isExit);
            }).Wait();
            
        }
        private string ReceivedMessage(string? line)
        {
            ReadOnlySpan<char> fullMessage = line.AsSpan();
            ReadOnlySpan<char> message = fullMessage.Slice(fullMessage.IndexOf(';') + 1);
            ClientMessageType clientMessageType = (ClientMessageType)byte.Parse(fullMessage.Slice(0, fullMessage.IndexOf(';')));


            switch (clientMessageType)
            {
                case ClientMessageType.Exit:
                    if(message.ToString() == "\\q")
                    {
                        _endChating = true;
                        Log($"[{_tcpClient.Client.RemoteEndPoint}] Left Server", SystemMessageType.System);
                        MessageReceive?.Invoke($"[{_tcpClient.Client.RemoteEndPoint}] [{NickName}]: Left Server");
                    }
                    break;
                case ClientMessageType.Message:



                    Log(message, SystemMessageType.Message);
                    MessageReceive?.Invoke($"[{_tcpClient.Client.RemoteEndPoint}] [{NickName}]: {message}");

                    break;
                case ClientMessageType.Information:
                    break;
                case ClientMessageType.Private:

                    if (message.StartsWith('+'))
                    {
                        PrivateRoom(Server.GetClient(message.Slice(1).Trim().ToString())!);
                    }
                    else
                    {
                        Log($"{NickName} {message}", SystemMessageType.Invite);

                        var client = Server.GetClient(message.Trim().ToString());
                        if (client != null && client != this)
                        {
                            client.SendMessage($"({NickName}) invite you to private chat send [ \\+ ] to accept", SystemMessageType.Invite);
                            PrivateRoom(client);
                        }
                        else
                            SendMessage($"Client {message} doesn`t exist", SystemMessageType.System);
                    }

                    break;

                default:
                    SendMessage(message, SystemMessageType.Unknown);
                    break;
            }
            Server.AddStory(clientMessageType, message.ToString());
            return message.ToString();
        }
        private void Log(ReadOnlySpan<char> message, SystemMessageType messageType) => Console.WriteLine($"[{messageType}] [{_tcpClient.Client.RemoteEndPoint}]: {message}");

        public void SendMessage(ReadOnlySpan<char> message, SystemMessageType messageType) => SendMessage(_writer, message, messageType);

        internal static void SendMessage(StreamWriter writer, ReadOnlySpan<char> message, SystemMessageType messageType)
        {
            switch (messageType)
            {
                case SystemMessageType.Unknown:
                    writer.WriteLine($"{(byte)messageType};[???]: {message}");
                    break;
                case SystemMessageType.Message:
                    writer.WriteLine($"{(byte)messageType};{message}");
                    break;
                case SystemMessageType.System:
                    writer.WriteLine($"{(byte)messageType};[System]: {message}");
                    break;
                case SystemMessageType.Invite:
                    writer.WriteLine($"{(byte)messageType};[Private invite]: {message}");
                    break;
                case SystemMessageType.Error:
                    writer.WriteLine($"{(byte)messageType};[!!System!!]: {message}");
                    break;
                case SystemMessageType.Private:
                    writer.WriteLine($"{(byte)messageType};[Private]: {message}");
                    break;
                default:
                    break;
            }

            writer.Flush();
        }

        public void Dispose() => _stream.Close();

        public Task<string> RegisterNickName()
        {
            return Task.Run(async () =>
            {
                SendMessage("Write login", SystemMessageType.System);
                string? nickName = await _reader.ReadLineAsync();
                NickName = nickName.AsSpan().Slice(2).ToString();
                return NickName;
            });
        }
        internal Task<bool> ConfirmPassword()
        {
            return Task.Run(async () =>
            {
                SendMessage("Write password", SystemMessageType.System);
                string? password = (await _reader.ReadLineAsync()).AsSpan().Slice(2).ToString();
                return Password.Equals(password);
            });
        }

        internal Task SetPassword()
        {
            return Task.Run(async () =>
            {
                SendMessage("Write password", SystemMessageType.System);
                string? password = (await _reader.ReadLineAsync()).AsSpan().Slice(2).ToString();
                if (password != null) Password = password;
                else Password = "1234";
            });
        }
        internal void ReSetTcp(TcpClient tcpClient, NetworkStream stream, StreamReader reader, StreamWriter writer)
        { 
            _tcpClient = tcpClient;
            _stream = stream;
            _reader = reader;
            _writer = writer;
        }
    }
}
