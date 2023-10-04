using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


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
        private string shop_no = string.Empty;
        private string auth_key = "C.ORDER";
        private string path_status = string.Empty;
        private string pos_extra = string.Empty;
        private string path_order = string.Empty;
        private string print_port = "COM6";
        private int print_speed = 9600;
        private int print_pin_width = 2;
        private int print_pin_height = 2;
        private int print_margin_pin_top = 3;
        private int print_margin_pin_bottom = 5;
        private int print_margin_order_top = 3;
        private int print_margin_order_bottom = 5;
        private bool print_use = true;
        private bool order_popup = true;
        private int timer_status_query = 30;
        readonly Config config = Config.Instance;
        JsonWrapper? lastTableStatus = null;
        public Dictionary<string, string> dicScdTable = new();
        public Dictionary<string, string> dicMenuName = new();
        public Dictionary<string, string> dicMenuPrice = new();

        ThemalPrint themalPrint = new();

        FormLogin? formLogin;
        FormTable? formTable;
        FormMenu? formMenu;
        FormOrder? formOrder;

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
                    Thread.Sleep(1);   // 메인 스레드와 다른 스레드이기 때문에 슬립을 사용해도 다른 시스템에는 영향이 없다.
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

        public void InitUI()
        {
            this.TopLevel = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(701, 44);

            this.Paint += MainForm_Paint;
            this.MouseDown += MainForm_MouseDown;
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
            if (config.GetString("order_popup") == "false")
            {
                order_popup = false;
            }
            if (config.GetString("print_use") == "false")
            {
                print_use = false;
            }

            print_port = config.GetString("print_port");
            print_speed = config.GetInt("print_speed");
            print_pin_width = config.GetInt("print_pin_width");
            print_pin_height = config.GetInt("print_pin_height");
            print_margin_pin_top = config.GetInt("print_margin_pin_top");
            print_margin_pin_bottom = config.GetInt("print_margin_pin_bottom");
            print_margin_order_top = config.GetInt("print_margin_order_top");
            print_margin_order_bottom = config.GetInt("print_margin_order_bottom");
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

            themalPrint.setConstant(print_port,
                print_speed,
                print_margin_pin_top,
                print_margin_pin_bottom,
                print_margin_order_top,
                print_margin_order_bottom);

            // Temporary Process;
            JsonWrapper jsonScdTable = new();
            if (jsonScdTable.Load(config.GetString("path_scdtable"), codepage: 51949) == true)
            {
                jsonScdTable.SetOptions(false);
                jsonScdTable.Parse();
            }

            var data = jsonScdTable.GetNode("TABLE").AsArray();
            dicScdTable.Clear();
            foreach (var node in data)
            {
                string table_nm = node!["TABLE_NM"]!.ToString();
                string table_cd = node!["TABLE_CD"]!.ToString();
                dicScdTable[table_nm] = table_cd;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitUI();
            InitData();
            this.ActiveControl = null;
            client = new Network(MessageHandler, StatusHandler);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.Handle != IntPtr.Zero)
            {
                IntPtr hWndDeskTop = GetDesktopWindow();
                SetParent(this.Handle, hWndDeskTop);
            }

            base.OnHandleCreated(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Debug("MainForm_Load");
            // ToDo: 기능 중단 
            this.Location = new Point(256, 17);
            Point parentPoint = this.Location;

            this.formLogin = new()
            {
                Location = parentPoint,
                mainForm = this,
                Owner = this,
                TopLevel = true
            };

            this.formTable = new()
            {
                Location = parentPoint,
                mainForm = this,
                Owner = this,
                Top = Top + 54,
                TopLevel = true,
            };

            this.formOrder = new()
            {
                mainForm = this,
                Location = parentPoint,
                Top = Top + 54,
                TopLevel = true
            };

            this.formOrder.Show();
            this.formOrder.Hide();

            this.formLogin.ShowDialog();

            globalKeyboardHook();
            Connect();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Verbose("MainForm_FormClosing");
            UnHook();
            await client.CloseAsync();
            //formLogin.Dispose();

            Log.CloseAndFlush();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Verbose("MainForm_FormClosed");
        }

        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
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
            this.formTable?.SetTableData();
            this.formTable?.ShowDialog();
        }

        private void picShowOrder_Click(object sender, EventArgs e)
        {
            string url = "http://corder.co.kr/ManagerOrder";
            util.Tools.Browse(url);
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
        }

        private void picLogo_DoubleClick(object sender, EventArgs e)
        {
            this.Location = new Point(256, 17);
        }

        private void picMenu_Click(object sender, EventArgs e)
        {
            if (formMenu?.Visible == true)
            {
                return;
            }

            formMenu = new()
            {
                Owner = this,
                TopLevel = true,
                mainForm = this
            };

            Point parentPoint = this.Location;
            formMenu.Location = parentPoint;
            formMenu.Top += 54;
            formMenu.Show();
        }
    }
}
