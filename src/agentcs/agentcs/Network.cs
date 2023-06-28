using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace agentcs
{
    public class Network
    {
        public delegate void Delegate(string message);
        Delegate callback;

        private ClientWebSocket clientWebSocket;

        public Network(Delegate func)
        {
            clientWebSocket = new ClientWebSocket();
            callback = func;
        }

        public async Task ConnectAsync(Uri uri)
        {
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            StartReceiving();
        }

        private void StartReceiving()
        {
            _ = Task.Run(ReceiveAsync);
        }

        private async Task ReceiveAsync()
        {
            byte[] buffer = new byte[1024];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }

                string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                callback(receivedMessage);
                //Console.WriteLine("Received message: " + receivedMessage);
            }
        }

        public async Task SendAsync(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task CloseAsync()
        {
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
