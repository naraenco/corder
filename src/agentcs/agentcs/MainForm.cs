using System.Diagnostics;

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
            config.load();

            Console.WriteLine(config.getString("server_address"));
            Console.WriteLine(config.getInt("timer_connect_retry"));
        }
    }
}