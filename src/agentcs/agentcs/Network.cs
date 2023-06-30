using System.Net.WebSockets;
using System.Text;


namespace agentcs
{
    public class Network 
    {
        public delegate void Delegate(string message);
        Delegate MessageHandler;
        bool connect_status = false;
        bool service_status = true;
        
        private ClientWebSocket clientWebSocket;

        public Network(Delegate func)
        {
            MessageHandler = func;
            clientWebSocket = null!;
        }

        public async Task ConnectAsync(Uri uri)
        {
            Console.WriteLine("Network.ConnectAsync()");
            while (service_status == true)
            {
                if (connect_status == false)
                {
                    try
                    {
                        clientWebSocket = new ClientWebSocket();
                        await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                        connect_status = true;
                        StartReceiving();
                    }
                    catch (WebSocketException ex)
                    {
                        connect_status = false;
                        Console.WriteLine("ConnectAsync.WebSocketException : {0}", ex.WebSocketErrorCode);
                        if (ex.WebSocketErrorCode == WebSocketError.Faulted)
                        {
                            Console.WriteLine("접속 안되거나 끊김 -_-");
                        }
                    }
                }
                await Task.Delay(10000);
            }
        }

        private void StartReceiving()
        {
            Console.WriteLine("Network.StartReceiving()");
            try
            {
                _ = Task.Run(ReceiveAsync);
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartReceiving.Exception : {0}", ex.Message);
            }
        }

        private async Task ReceiveAsync()
        {
            Console.WriteLine("Network.ReceiveAsync()");

            byte[] buffer = new byte[1024];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                if (connect_status == false) return;
                try
                {
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WebSocketMessageType.Close");
                        await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }

                    string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //Console.WriteLine("Received message: " + receivedMessage);
                    MessageHandler(receivedMessage);
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine("ReceiveAsync.WebSocketException : {0}", ex.WebSocketErrorCode);
                    if (ex.WebSocketErrorCode == WebSocketError.Faulted)
                    {
                        Console.WriteLine("접속이 끊긴거야?");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ReceiveAsync.Exception : {0}", ex.Message);
                }
            }
        }

        public async Task SendAsync(string message)
        {
            Console.WriteLine("Network.SendAsync()");

            if (connect_status == false) return;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendAsync.Exception : {0}", ex.Message);
            }
        }

        public async Task CloseAsync()
        {
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
