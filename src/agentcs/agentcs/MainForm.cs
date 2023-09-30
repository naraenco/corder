using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;

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
        private string shop_no = "";
        private string auth_key = "C.ORDER";
        private string path_status = "";
        private string pos_extra = "";
        private string path_order = "";
        private string print_port = "COM6";
        private int print_speed = 9600;
        private int print_pin_width = 2;
        private int print_pin_height = 2;
        private bool print_use = true;
        private int timer_status_query = 30;
        readonly Config config = Config.Instance;
        JsonWrapper? lastTableStatus = null;
        public Dictionary<string, string> dicScdTable = new();
        public Dictionary<string, string> dicMenuName = new();
        public Dictionary<string, string> dicMenuPrice = new();

        FormLogin? formLogin;
        FormTable? formTable;

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

        public void SetDialog(int style)
        {
            //Log.Debug($"SetDialog {style}");

            switch (style)
            {
                case 0: // login
                    picGenPin.Visible = false;
                    picTableStatus.Visible = false;
                    picShowOrder.Visible = false;
                    break;

                case 1: // default
                    picGenPin.Visible = true;
                    picTableStatus.Visible = true;
                    picShowOrder.Visible = true;
                    break;
            }
        }

        public void InitUI()
        {
            this.TopLevel = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(701, 44);

            this.Paint += MainForm_Paint;
            this.MouseDown += MainForm_MouseDown;

            //nanumFont = new Font(FontManager.fontFamilys[0], 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            // default
            picLogo.Location = new Point(49, 15);

            // func
            picGenPin.Location = new Point(306, 7);
            picTableStatus.Location = new Point(436, 7);
            picShowOrder.Location = new Point(566, 7);

            // ToDo: UI 설정
            //this.Controls.Add(this.formLogin);
            //this.formLogin.Show();
            //this.formLogin.ShowDialog();
            //this.formLogin.Location = new Point(0, 0);
            //SetDialog(0);
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
            print_speed = config.GetInt("print_speed");
            print_pin_width = config.GetInt("print_pin_width");
            print_pin_height = config.GetInt("print_pin_height");
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

            this.formLogin = new()
            {
                Owner = this,
                TopLevel = true,
                mainForm = this
            };

            this.formTable = new()
            {
                Owner = this,
                TopLevel = true,
                mainForm = this
            };

            Point parentPoint = this.Location;
            this.formTable.Location = parentPoint;
            this.formTable.Top += 54;
            this.formLogin.Location = parentPoint;
            //this.formLogin.ShowDialog();

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
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            //System.Drawing.Graphics formGraphics;
            //formGraphics = this.CreateGraphics();
            //formGraphics.FillRectangle(myBrush, new Rectangle(400, 2, 398, 396));
            //myBrush.Dispose();
            //formGraphics.Dispose();

            //using (Pen pen = new Pen(Color.FromArgb(255, 225, 225, 225), 2))
            //{
            //    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            //}
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
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
            string command = $"start http://corder.co.kr/manager";

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = string.Format("/C {0}", command);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }

        private void picLogo_DoubleClick(object sender, EventArgs e)
        {
            this.Location = new Point(256, 17);
        }

        private void picMenu_Click(object sender, EventArgs e)
        {
            FormMenu formMenu = new()
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
