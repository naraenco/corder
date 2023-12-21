using System.Diagnostics;
using System.Text.Json.Nodes;

namespace COrderUpdater
{
    public partial class MainForm : Form
    {
        readonly static string APPNAME = "corderagent.exe";
        readonly static string FULLPACK = "fullpack.exe";
        readonly static string UPDATEPACK = "updatepack.exe";

        public string server_address = "127.0.0.1";
        public string server_port = "19000";
        public string current_version = String.Empty;
        public string path = String.Empty;
        public string api_url = String.Empty;

        readonly Config config = Config.Instance;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lbMessage.Text = "������ Ȯ�����Դϴ�";

            if (config.Load() == false)
                return;

            server_address = config.GetString("server_address");
            api_url = "http://" + server_address + ":" + server_port + "/agent-version";

            CheckVersion();
        }

        public async void CheckVersion()
        {
            string? response = await CallApiAsync(api_url);
            if (response == null)
            {
                lbMessage.Text = "�������� ���� ������ ���� �� �����ϴ�.";
                MessageBox.Show("�������� ���� ������ ���� �� �����ϴ�.");
                return;
            }

            JsonNode? node = JsonNode.Parse(response!);
            if (node == null)
            {
                lbMessage.Text = "�������� ���� ������ ���� �� �����ϴ�.";
                MessageBox.Show("�������� ���� ������ ���� �� �����ϴ�.");
                return;
            }
            current_version = node["version"]!.ToString();
            lbCurrentVersion.Text = current_version;
            lbFileVersion.Text = "";
            path = Directory.GetCurrentDirectory() + "\\" + APPNAME;
            if (File.Exists(path) == false)
            {
                // �ű� �ٿ�ε� 
                lbFileVersion.Text = "";
                lbMessage.Text = "������Ʈ ������ �����ϴ�. �ű� ��ġ�� �����մϴ�";
                await DownloadAgent(FULLPACK);
                lbMessage.Text = "�ٿ�ε� �Ϸ�";
                path = Directory.GetCurrentDirectory() + "\\" + FULLPACK;
                if (File.Exists(path))
                {
                    Process.Start(path, "");
                }
                Close();
                return;
            }

            FileVersionInfo agentInfo = FileVersionInfo.GetVersionInfo(path);
            if (agentInfo != null)
            {
                lbFileVersion.Text = agentInfo.FileVersion;

                if (current_version != agentInfo.FileVersion)
                {
                    lbMessage.Text = "������Ʈ�� �����մϴ�";
                    path = Directory.GetCurrentDirectory() + "\\" + UPDATEPACK;
                    UpdateAgent();
                    return;
                }
                else
                {
                    lbMessage.Text = "������Ʈ�� �ֽ� �����Դϴ�";
                    path = Directory.GetCurrentDirectory() + "\\" + APPNAME;
                    Process.Start(path, "C.ORDER");
                    Close();
                    return;
                }
            }
            else
            {
                lbMessage.Text = "������ Ȯ���� �� �����ϴ�";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
