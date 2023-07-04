using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.WebSockets;
using System.Windows.Forms;
using System.Threading;
using Serilog;
using Serilog.Core;
using System.Runtime.InteropServices;
using System.Reflection.Emit;

namespace agentcs
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        static public bool Shift { get; private set; }
        static public bool Control { get; private set; }

        private IntPtr hook = IntPtr.Zero;


        private Network client;
        Thread? tsThread;
        private string shop_no = "";
        private string auth_key = "C.ORDER";
        private string path_status = "";
        private string pos_extra = "";
        private string path_order = "";
        private string print_port = "COM6";
        private int print_font_width = 0;
        private int print_font_height = 0;
        private int timer_status_query = 30;
        bool table_status = true;
        readonly Config config = Config.Instance;


        public MainForm()
        {
            InitializeComponent();
            InitData();
            client = new Network(MessageHandler, StatusHandler);
        }

        private void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        private void UnHook()
        {
            UnhookWindowsHookEx(hook);
        }

        public IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            Log.Verbose("hookProc IN");

            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                Log.Verbose("hookProc 01");

                Keys vkKey = (Keys)Marshal.ReadInt32(lParam);
                //int keyCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine("code : {0}, vkKey : {1}", keyCode, vkKey);
                Log.Verbose("hookProc 02");

                if (vkKey == Keys.LControlKey)
                {
                    Control = true;
                }

                if (vkKey == Keys.LShiftKey)
                {
                    Shift = true;
                }
                Log.Verbose("hookProc 03");
            }

            if (wParam == (IntPtr)WM_SYSKEYDOWN) // 알트 키 눌림
            {
                Log.Verbose("hookProc 04");
                int key = Marshal.ReadInt32(lParam);
                Console.WriteLine(key);

                Log.Verbose("hookProc 05");

                if (key == 221)
                {
                    Log.Verbose("hookProc 06");
                    MessageBox.Show("기능키 ALT + SHIFT + ] 가 눌렸습니다.");
                    GenPinReq();
                }
            }

            if (wParam == (IntPtr)WM_KEYUP)
            {
                Control = false;
                Shift = false;
                return IntPtr.Zero;
            }
            Log.Verbose("hookProc 07");
            return CallNextHookEx(hook, code, (int)wParam, lParam);
        }

        public void InitData()
        {
            if (config.Load() == false)
                return;
            path_status = config.GetString("path_status");
            pos_extra = config.GetString("pos_extra") + ".json";
            path_order = config.GetString("path_order") + "Order_";
            shop_no = config.GetString("shop_no");
            auth_key = config.GetString("auth_key");
            print_port = config.GetString("print_port");
            print_font_width = config.GetInt("print_font_width");
            print_font_height = config.GetInt("print_font_height");
            timer_status_query = config.GetInt("timer_status_query");
            if (timer_status_query < 10)
            {
                timer_status_query = 10;
            }
            timer_status_query *= 1000;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("log/log_.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Debug("MainForm_Load");
            SetHook();
            Connect();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Verbose("MainForm_FormClosing");
            UnHook();
            table_status = false;
            tsThread?.Join();
            await client.CloseAsync();
            Log.CloseAndFlush();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Verbose("MainForm_FormClosed");
        }

        private void buttonGenPin_Click(object sender, EventArgs e)
        {
            Log.Information("buttonGenPin_Click");
            GenPinReq();
        }

        public void StatusHandler(WebSocketError error)
        {
            switch (error)
            {
                case WebSocketError.Success:
                    Log.Information("StatusHandler : CONNECTED");
                    LoginReq();
                    break;

                case WebSocketError.Faulted:
                    Log.Information("StatusHandler : FAULTED");
                    break;

                //case WebSocketError.InvalidState:
                //    break;

                default:
                    Log.Information("StatusHandler : DISCONNECTED");
                    break;
            }
        }

        public void MessageHandler(string message)
        {
            Log.Debug("MessageHandler : " + message);

            try
            {
                var json = JsonNode.Parse(message);
                json = json?.Root;
                if (json == null) return;

                string msgtype = json["msgtype"]!.ToString();

                switch (msgtype)
                {
                    case "login":
                        Log.Verbose("MessageHandler.login");
                        // ToDo: 로그인에 성공하면 POS 데이터 전송
                        SendPosData();
                        tsThread = new(QueryTableStatus);
                        tsThread.Start();
                        break;

                    case "genpin":
                        Log.Verbose("MessageHandler.genpin");
                        // ToDo: 핀 생성 성공하면 감열식 프린터로 인쇄
                        GenPinAns(json);
                        break;

                    case "order":
                        Log.Verbose("MessageHandler.order");
                        OrderAns(json);
                        break;

                    case "mylist":
                        Log.Verbose("MessageHandler.mylist");
                        break;

                    default:
                        Log.Warning("Unknown Message Type");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("MessageHandler : {0}", ex.Message);
            }
        }

        public async void SendPosData()
        {
            Log.Information("SendPosData()");

            JsonWrapper jsonTableMap = new();
            if (jsonTableMap.Load(config.GetString("path_tablemap"), codepage: 51949) == true)
            {
                jsonTableMap.SetOptions(false);
                jsonTableMap.Parse();
            }

            JsonWrapper jsonMenu = new();
            if (jsonMenu.Load(config.GetString("path_menu"), codepage: 51949) == true)
            {
                jsonMenu.SetOptions(false);
                jsonMenu.Parse();
            }

            string message = "{\"msgtype\":\"tablemap\",\"shop_no\": \""
                + shop_no
                + "\", \"data\":"
                + jsonTableMap.ToString()
                + "}";
            await client.SendAsync(message);

            message = "{\"msgtype\":\"menu\",\"shop_no\": \""
                + shop_no
                + "\", \"data\":"
                + jsonMenu.ToString()
                + "}";
            await client.SendAsync(message);
        }

        public async void Connect()
        {
            string address = "ws://" + config.GetString("server_address")
                + ":" + config.GetString("server_port") + "/ws";
            Uri serverUri = new(address);
            Log.Information("Server Address : " + address);
            await client.ConnectAsync(serverUri);
        }

        public async void LoginReq()
        {
            Log.Information("LoginReq()");

            try
            {
                string auth_key = "C.오더";
                string message = "{\"msgtype\":\"login\",\"shop_no\": \""
                    + shop_no + "\",\"auth_key\":\""
                    + auth_key + "\"}";
                await client.SendAsync(message);
                Log.Debug("LoginReq : " + message);
            }
            catch (Exception ex)
            {
                Log.Error("LoginReq : {0}", ex.Message);
            }
        }

        public async void GenPinReq()
        {
            string message = "{\"msgtype\":\"genpin\",\"shop_no\": \"" + shop_no + "\"}";
            await client.SendAsync(message);
            Log.Debug("GenPinReq : " + message);
            
        }

        public void GenPinAns(JsonNode node)
        {
            Log.Information("GenPinAns()");

            try
            {
                string pin = node["pin"]!.ToString();
                string createdAt = node["created_at"]!.ToString();
                if (String.IsNullOrEmpty(pin))
                {
                    Log.Warning("pin is null or empty");
                    return;
                }
                ThemalPrint print = new();
                print.PrintPin(print_port,
                    pin,
                    createdAt,
                    print_font_width,
                    print_font_height);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public void OrderAns(JsonNode node)
        {
            Log.Information("OrderAns()");

            try
            {
                string regdate = node["regdate"]!.ToString();
                string path = path_order + regdate + pos_extra;
                JsonNode orderList = node["pos_order"]!["orderList"]!;

                node["status"] = 1;
                Console.WriteLine(node.ToJsonString());

                JsonObject obj = new();
                obj.Add("tableNo", node["table_cd"]?.ToString());
                obj.Add("orderList", JsonNode.Parse(orderList.ToJsonString()));

                JsonUtil.WriteFile(path, obj, indent: true, codepage: 51949);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async void QueryTableStatus()
        {
            while (table_status == true)
            {
                Log.Information("QueryTableStatus()");

                JsonWrapper jsonTableStatus = new();
                if (jsonTableStatus.Load(config.GetString("path_status"), codepage: 51949) == true)
                {
                    jsonTableStatus.SetOptions(false);
                    jsonTableStatus.Parse();
                }

                string message = "{\"msgtype\":\"tablestatus\",\"shop_no\": \""
                    + shop_no
                    + "\", \"data\":"
                    + jsonTableStatus.ToString()
                    + "}";
                await client.SendAsync(message);

                await Task.Delay(timer_status_query);
            }
        }
    }
}
