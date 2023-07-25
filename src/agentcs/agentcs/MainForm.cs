using Serilog;
using System.Runtime.InteropServices;
using System.Text;


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

        Font? nanumFont;
        FormLogin? formLogin;

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

            nanumFont = new Font(FontManager.fontFamilys[0], 14, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));


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

            // etc
            textTable.Font = nanumFont;

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
            //formLogin.Dispose();

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
