namespace agentcs
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Config config = Config.Instance;
            config.Load();

            //Console.WriteLine(config.GetString("server_address"));
            //Console.WriteLine(config.GetInt("timer_connect_retry"));

            PosData pos = new();
            pos.LoadJsonFiles();
        }
    }
}