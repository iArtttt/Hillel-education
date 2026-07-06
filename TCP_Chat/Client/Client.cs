using Server.Lib;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Client : IDisposable
    {
        private bool _isEnd = false;
        private bool _isInPrivateChat = false;
        private string _invitorNickName = string.Empty;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        public Client()
        {
            _tcpClient = new TcpClient(AddressFamily.InterNetwork);
            IPEndPoint ep = Search().Result;
            _tcpClient.Connect(/*"localhost", 5002*/ep);
            Console.WriteLine($"Client started on: {_tcpClient.Client.LocalEndPoint}");
            Console.WriteLine($"Client connected: {_tcpClient.Client.RemoteEndPoint}");
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
        }
        public async Task<IPEndPoint> Search()
        {
            ServerScan scan = new ServerScan();
            return await scan.ScanAsync();
        }
        public void Start()
        {

            var readerTask = Task.Run(ReaderTask);
            var writerTask = Task.Run(WriterTask);

            Task.WaitAll(readerTask, writerTask);
        }
        private void ReaderTask()
        {
            string? line = null;
            do
            {
                line = _reader.ReadLine();

                var span = line.AsSpan();
                CheckReceivedMessageType(span);

            } while (!_isEnd);


        }
        private void WriterTask()
        {
            string line = string.Empty;
            do
            {
                line = CheckBeenSentMessage(Console.ReadLine().AsSpan());

                _writer.WriteLine(line);
                _writer.Flush();

            } while (!_isEnd);
        }

        private string CheckBeenSentMessage(ReadOnlySpan<char> message)
        {
            if (message.Length == 0) return $"{(byte)ClientMessageType.Exit};";
            if (message.StartsWith('\\') && message.Length > 1)
            {
                var command = message.Slice(1,1);
                switch (command)
                {
                    case "p":
                        return $"{(byte)ClientMessageType.Private};{message.Slice(2)}";
                    case "+":
                        return $"{(byte)ClientMessageType.Private};+{_invitorNickName??=string.Empty}";
                    case "h":
                        return $"{(byte)ClientMessageType.Information};{message.Slice(2)}";
                    case "i":
                        return $"{(byte)ClientMessageType.Information};{message.Slice(2)}";
                    case "q":
                        _isEnd = true;
                        return $"{(byte)ClientMessageType.Exit};{message}";
                    default:
                        return $"{(byte)ClientMessageType.Information};{message.Slice(2)}";
                }
            }
            else
                return $"{(byte)ClientMessageType.Message};{message}";
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _tcpClient?.Dispose();
        }

        private ReadOnlySpan<char> CheckReceivedMessageType(ReadOnlySpan<char> messageSpan)
        {
            int indexOfSpan = messageSpan.IndexOf(";");
            SystemMessageType messageType = (SystemMessageType)byte.Parse(messageSpan.Slice(0, indexOfSpan));
            ReadOnlySpan<char> mainMessage = messageSpan.Slice(indexOfSpan + 1).Trim();

            switch (messageType)
            {

                case SystemMessageType.Unknown:
                    Writer.SuccessWriteline(mainMessage.ToString());
                    break;
                case SystemMessageType.Message:

                    ReadOnlySpan<char> endPoint = mainMessage.Slice(1, mainMessage.IndexOf(']') - 1);
                    ReadOnlySpan<char> message = mainMessage.Slice(mainMessage.IndexOf(']') + 1).Trim();

                    if (!_isInPrivateChat && endPoint.ToString() != _tcpClient.Client.LocalEndPoint.ToString())
                        Console.WriteLine(message);

                    break;
                case SystemMessageType.System:
                    Writer.InfoWriteline(mainMessage.ToString());
                    break;
                case SystemMessageType.Error:
                    Writer.ErrorWriteline(mainMessage.ToString());
                    break;
                case SystemMessageType.Invite:
                    int start = mainMessage.IndexOf('(');
                    int end = mainMessage.IndexOf(')');

                    _invitorNickName = mainMessage.Slice(start + 1, (end - 1) - start).ToString();
                    Writer.BlueWriteline(mainMessage.ToString());
                    break;
                case SystemMessageType.Private:
                    if (mainMessage.ToString() == "You are in Private chat now")
                    {
                        _isInPrivateChat = true;
                        Writer.BlueWriteline(mainMessage.ToString());
                    }
                    if (mainMessage.StartsWith("[Private]: --Exit"))
                    {
                        Writer.SuccessWriteline(mainMessage.Slice(17).ToString());
                        _writer.WriteLine($"{(byte)ClientMessageType.Exit};");
                        _writer.Flush();
                        _invitorNickName = string.Empty;
                        _isInPrivateChat = false;
                    }
                    else
                        Writer.BlueWriteline(mainMessage.ToString());
                    break;
            }


            return messageSpan;
        }
    }
}
