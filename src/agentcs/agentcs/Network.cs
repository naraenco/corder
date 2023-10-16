using System.Net.WebSockets;
using System.Text;
using Serilog;
using Serilog.Core;


namespace agentcs
{
    public class Network 
    {
        public delegate void Delegate1(string message);
        public delegate void Delegate2(WebSocketError error);
        Delegate1 MessageHandler;
        Delegate2 StatusHandler;
        public bool connect_status = false;
        bool service_status = true;
        int timer_connect_retry;
        readonly Config config = Config.Instance;

        private ClientWebSocket clientWebSocket;

        public Network(Delegate1 func1, Delegate2 func2)
        {
            MessageHandler = func1;
            StatusHandler = func2;
            clientWebSocket = null!;
            timer_connect_retry = config.GetInt("timer_connect_retry");
            if (timer_connect_retry < 10)
            {
                timer_connect_retry = 10;
            }
            timer_connect_retry *= 1000;
        }

        ~Network()
        {
            service_status = false;
        }

        public async Task ConnectAsync(Uri uri)
        {
            Log.Information("Network.ConnectAsync()");

            while (service_status == true)
            {
                if (connect_status == false)
                {
                    //if (clientWebSocket != null)
                    //    clientWebSocket.Abort();

                    try
                    {
                        clientWebSocket = new ClientWebSocket();
                        await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                        StartReceiving();
                        StatusHandler(WebSocketError.Success);
                    }
                    catch (WebSocketException ex)
                    {
                        connect_status = false;
                        StatusHandler(ex.WebSocketErrorCode);
                        Console.WriteLine("ConnectAsync.WebSocketException : {0}", ex.WebSocketErrorCode);
                    }
                }
                await Task.Delay(timer_connect_retry);
            }
        }

        private void StartReceiving()
        {
            Log.Information("Network.StartReceiving()");
            try
            {
                _ = Task.Run(ReceiveAsync);
            }
            catch (Exception ex)
            {
                Log.Error("StartReceiving.Exception : {0}", ex.Message);
            }
        }

        private async Task ReceiveAsync()
        {
            Log.Information("Network.ReceiveAsync()");

            byte[] buffer = new byte[8192];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                if (connect_status == false) return;
                try
                {
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        //Console.WriteLine("WebSocketMessageType.Close");
                        //await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        connect_status = false;
                        break;
                    }

                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Received message: " + receivedMessage);
                    MessageHandler(receivedMessage);
                }
                catch (WebSocketException ex)
                {
                    connect_status = false;
                    StatusHandler(ex.WebSocketErrorCode);
                    Log.Error("ReceiveAsync.WebSocketException : {0}", ex.WebSocketErrorCode);
                }
                catch (Exception ex)
                {
                    Log.Error("ReceiveAsync.Exception : {0}", ex.Message);
                }
            }
        }

        public async Task SendAsync(string message)
        {
            Log.Information("Network.SendAsync()");

            if (connect_status == false) return;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Log.Error("SendAsync.Exception : {0}", ex.Message);
            }
        }

        public async Task CloseAsync()
        {
            Log.Information("Network.CloseAsync()");
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
