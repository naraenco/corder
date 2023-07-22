using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.WebSockets;
using System.Threading;
using Serilog;
using Serilog.Core;
using System.Runtime.InteropServices;
using System.Collections;
using System.DirectoryServices;
using System.Text;
using System.Net;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;


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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc? hookProcDelegate;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2; const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        static public bool Shift { get; private set; }
        static public bool Control { get; private set; }

        private IntPtr hook = IntPtr.Zero;

        FileSystemWatcher watcher = new();
        private Network client;
        const string apiUrl = "http://corder.co.kr/api/";
        private string shop_no = "";
        private string auth_key = "C.ORDER";
        private string path_status = "";
        private string pos_extra = "";
        private string path_order = "";
        private string print_port = "COM6";
        private bool print_use = true;
        private int print_font_width = 0;
        private int print_font_height = 0;
        private int timer_status_query = 30;
        readonly Config config = Config.Instance;
        JsonWrapper? lastTableStatus = null;

        FormLogin formLogin;

        public void globalKeyboardHook()
        {
            hookProcDelegate = hookProc;
            SetHook();
        }

        private void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProcDelegate!, hInstance, 0);
        }

        private void UnHook()
        {
            UnhookWindowsHookEx(hook);
        }

        public IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys vkKey = (Keys)Marshal.ReadInt32(lParam);
                //int keyCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine("code : {0}, vkKey : {1}", keyCode, vkKey);

                if (vkKey == Keys.LControlKey)
                {
                    Control = true;
                }

                if (vkKey == Keys.LShiftKey)
                {
                    Shift = true;
                }
            }

            if (wParam == (IntPtr)WM_SYSKEYDOWN) // 알트 키 눌림
            {
                int key = Marshal.ReadInt32(lParam);
                //Console.WriteLine(key);

                if (key == 221)
                {
                    //MessageBox.Show("기능키 ALT + SHIFT + ] 가 눌렸습니다.");
                    GenPinReq();
                }
            }

            if (wParam == (IntPtr)WM_KEYUP)
            {
                Control = false;
                Shift = false;
                return IntPtr.Zero;
            }
            return CallNextHookEx(hook, code, (int)wParam, lParam);
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;

            try
            {
                watcher.EnableRaisingEvents = false;

                // 파일 쓰기 중인지 확인한다.
                while (true)
                {
                    try
                    {
                        using (Stream stream = File.Open(path_status, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            if (null != stream) break;
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                    finally
                    {
                    }
                    System.Threading.Thread.Sleep(1);   // 메인 스레드와 다른 스레드이기 때문에 슬립을 사용해도 다른 시스템에는 영향이 없다.
                }
                SendTableStatus();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        public void StatusMonitor()
        {
            watcher.Path = Path.GetDirectoryName(path_status)!;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += watcher_Changed;
            watcher.Filter = Path.GetFileName(path_status);
            watcher.IncludeSubdirectories = false;  // 하위 디렉토리의 변화까지 확인할 것이다.
            watcher.EnableRaisingEvents = true;     // 이벤트를 발생 할 것이다.
        }

        public void SetDialog(int style)
        {
            Console.WriteLine($"SetDialog : {style}");
            //Log.Debug($"SetDialog {style}");

            switch (style)
            {
                case 0: // login
                    buttonCancel.Visible = false;
                    textTable.Visible = false;
                    picGenPin.Visible = false;
                    picTableStatus.Visible = false;
                    picSetting.Visible = false;
                    picWebpage.Visible = false;
                    break;

                case 1: // default
                    buttonCancel.Visible = true;
                    textTable.Visible = true;
                    picGenPin.Visible = true;
                    picTableStatus.Visible = true;
                    picSetting.Visible = true;
                    picWebpage.Visible = true;
                    break;
            }
        }

        public void InitUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(800, 400);

            this.Paint += MainForm_Paint;
            this.MouseDown += MainForm_MouseDown;

            // default
            picClose.Location = new Point(340, 20);
            picLogo.Location = new Point(150, 32);
            picCorderText.Location = new Point(98, 60);
            picDecoBox.Location = new Point(370, 165);
            picDecoBox.BackColor = Color.Transparent;

            // func
            picGenPin.Location = new Point(58, 108);
            picTableStatus.Location = new Point(202, 108);
            picSetting.Location = new Point(58, 253);
            picWebpage.Location = new Point(202, 253);

            SetDialog(0);

            this.formLogin = new()
            {
                TopLevel = false
            };
            //this.formLogin.Parent = this;
            this.Controls.Add(this.formLogin);
            this.formLogin.Show();
            this.formLogin.Location = new Point(0, 0);
        }

        public void InitData()
        {
            if (config.Load() == false)
                return;
            path_status = config.GetString("path_status");
            pos_extra = config.GetString("pos_extra");
            path_order = config.GetString("path_order") + "Order_";
            shop_no = config.GetString("shop_no");
            auth_key = config.GetString("auth_key");
            if (config.GetString("print_use") == "false")
            {
                print_use = false;
            }
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

        public static async Task<string?> CallApiAsync(string apiUrl)
        {
            using HttpClient client = new();
            try
            {
                var buffer = await client.GetByteArrayAsync(apiUrl);
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                return responseString;
            }
            catch (HttpRequestException e)
            {
                Log.Error($"CallApiAsync Exception : {e.Message}");
                return null;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitUI();
            InitData();
            client = new Network(MessageHandler, StatusHandler);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Debug("MainForm_Load");
            // ToDo: 기능 중단 
            globalKeyboardHook();
            Connect();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Verbose("MainForm_FormClosing");
            UnHook();
            await client.CloseAsync();
            Log.CloseAndFlush();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Verbose("MainForm_FormClosed");
        }

        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(400, 2, 398, 396));
            myBrush.Dispose();
            formGraphics.Dispose();

            using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void MainForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IntPtr handle = this.Handle;
                ReleaseCapture();
                SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Log.Information("buttonCancel_Click()");
            SendClear(0, textTable.Text);
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
                        SendPosData();
                        SendTableStatus();
                        StatusMonitor();
                        break;

                    case "genpin":
                        Log.Verbose("MessageHandler.genpin");
                        if (print_use != false) GenPinAns(json);
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

            JsonWrapper jsonScdTable = new();
            if (jsonScdTable.Load(config.GetString("path_scdtable"), codepage: 51949) == true)
            {
                jsonScdTable.SetOptions(false);
                jsonScdTable.Parse();
            }

            JsonWrapper jsonUseTable = new();
            if (jsonUseTable.Load(config.GetString("path_usetable"), codepage: 51949) == true)
            {
                jsonUseTable.SetOptions(false);
                jsonUseTable.Parse();
            }

            JsonWrapper jsonMenu = new();
            if (jsonMenu.Load(config.GetString("path_menu"), codepage: 51949) == true)
            {
                jsonMenu.SetOptions(false);
                jsonMenu.Parse();
            }

            JsonWrapper jsonTouchClass = new();
            if (jsonTouchClass.Load(config.GetString("path_touchclass"), codepage: 51949) == true)
            {
                jsonTouchClass.SetOptions(true);
                jsonTouchClass.Parse();
            }

            JsonWrapper jsonTouchKey = new();
            if (jsonTouchKey.Load(config.GetString("path_touchkey"), codepage: 51949) == true)
            {
                jsonTouchKey.SetOptions(true);
                jsonTouchKey.Parse();
            }

            string message = "{\"msgtype\":\"tablemap\",\"shop_no\": \""
                + shop_no
                + "\", \"scdtable\":"
                + jsonScdTable.ToString()
                + ", \"usetable\":"
                + jsonUseTable.ToString()
                + "}";
            await client.SendAsync(message);

            string url1 = apiUrl + "sync_tables.php?shop_no=" + shop_no;
            try
            {
                string? response = await CallApiAsync(url1);
                Log.Debug("sync_tables response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"sync_tables exception : {ex.Message}");
            }

            message = "{\"msgtype\":\"menu\",\"shop_no\": \""
                + shop_no
                + "\", \"touch_class\":"
                + jsonTouchClass.ToString()
                + ", \"touch_key\":"
                + jsonTouchKey.ToString()
                + ", \"data\":"
                + jsonMenu.ToString()
                + "}";
            await client.SendAsync(message);

            string url2 = apiUrl + "sync_menus.php?shop_no=" + shop_no;
            try
            {
                string? response = await CallApiAsync(url2);
                Log.Debug("sync_menus response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"sync_menus exception : {ex.Message}");
            }
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
                string orderno = node["order_no"]!.ToString();
                string regdate = node["regdate"]!.ToString();
                string path = path_order + regdate + pos_extra + orderno + ".json";
                JsonNode orderList = node["pos_order"]!["orderList"]!;

                node["status"] = 1;
                //Console.WriteLine(node.ToJsonString());

                JsonObject obj = new();
                obj.Add("tableNo", node["table_cd"]?.ToString());
                obj.Add("orderList", JsonNode.Parse(orderList.ToJsonString()));
                JsonUtil.WriteFile(path, obj, indent: true, codepage: 51949);
                Log.Debug(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async void SendTableStatus()
        {
            JsonWrapper jsonTableStatus = new();
            if (jsonTableStatus.Load(path_status, codepage: 51949) == true)
            {
                jsonTableStatus.SetOptions(false);
                jsonTableStatus.Parse();
            }

            if (!String.IsNullOrEmpty(lastTableStatus?.ToString()) && jsonTableStatus.ToString() == lastTableStatus?.ToString())
            {
                return;
            }

            Log.Information("SendTableStatus()");

            try
            {
                List<string> orders = new();

                JsonArray currTables = jsonTableStatus.GetNode("tableList").AsArray();
                foreach (var table in currTables)
                {
                    var obj = table?.AsObject();
                    if (obj?.ContainsKey("orderList") == true)
                    {
                        string tableNo = table?["tableNo"]!.ToString()!;
                        orders?.Add(tableNo);
                    }
                }

                if (lastTableStatus != null)
                {
                    JsonArray lastTables = lastTableStatus.GetNode("tableList").AsArray();
                    foreach (var table in lastTables)
                    {
                        var obj = table?.AsObject();
                        if (obj?.ContainsKey("orderList") == true)
                        {
                            string tableNo = table?["tableNo"]!.ToString()!;
                            if (orders.Contains(tableNo) == false)
                            {
                                SendClear(1, tableNo);
                            }
                        }
                    }
                }

                lastTableStatus = new();
                lastTableStatus.Parse(jsonTableStatus.ToString()!);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            string message = "{\"msgtype\":\"tablestatus\",\"shop_no\": \""
                    + shop_no
                    + "\", \"data\":"
                    + jsonTableStatus.ToString()
                    + "}";
            await client.SendAsync(message);
        }

        public async void SendClear(int type, string table_cd)
        {
            Log.Information("SendClear() - type : {0}, table : {1}", type, table_cd);

            string message = "{\"msgtype\":\"clear\",\"shop_no\": \""
                    + shop_no
                    + "\", \"table_cd\": \""
                    + table_cd
                    + "\", \"type\": "
                    + type
                    + "}";
            Console.WriteLine(message);
            await client.SendAsync(message);

            string url = apiUrl + $"delete_connect.php?shop_no={shop_no}&table_cd={table_cd}";
            try
            {
                string? response = await CallApiAsync(url);
                Log.Debug("delete_connect response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"delete_connect exception : {ex.Message}");
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picGenPin_Click(object sender, EventArgs e)
        {
            Log.Information("picGenPin_Click");
            GenPinReq();
        }

        private void picTableStatus_Click(object sender, EventArgs e)
        {

        }

        private void picSetting_Click(object sender, EventArgs e)
        {

        }

        private void picWebpage_Click(object sender, EventArgs e)
        {

        }
    }
}
