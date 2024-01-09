using System.Diagnostics;
using System.Text.Json.Nodes;

namespace COrderUpdater
{
    public partial class MainForm : Form
    {
        readonly static string APPNAME = "corderagent.exe";
        readonly static string FULLPACK = "fullpack.exe";
        public static string update_url = String.Empty;
        public static string api_url = String.Empty;

        public string path = String.Empty;
        public string current_version = String.Empty;

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

            update_url = config.GetString("update_url");
            api_url = "http://" + config.GetString("server_address") + ":" + config.GetString("server_port") + "/agent-version";
            path = Directory.GetCurrentDirectory() + "\\" + APPNAME;

            CheckVersion();
        }

        public async void CheckVersion()
        {
            string? response = await CallApiAsync(api_url);
            if (response == null)
            {
                lbMessage.Text = "������ �������� �ʽ��ϴ�.";
                MessageBox.Show("������ �������� �ʽ��ϴ�.");
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
            bool bUpdate = false;


            if (File.Exists(path) == false)
            {
                lbFileVersion.Text = "";
                lbMessage.Text = "������Ʈ ������ �����ϴ�. �ű� ��ġ�� �����մϴ�";
                bUpdate = true;
            }
            else
            {
                FileVersionInfo agentInfo = FileVersionInfo.GetVersionInfo(path);
                if (agentInfo != null)
                {
                    lbFileVersion.Text = agentInfo.FileVersion;
                    if (current_version != agentInfo.FileVersion)
                    {
                        bUpdate = true;
                    }
                    else
                    {
                        bUpdate = false;
                    }
                }
                else
                {
                    lbMessage.Text = "������ Ȯ���� �� �����ϴ�";
                    return;
                }
            }

            if (bUpdate == true)
            {
                lbMessage.Text = "��ġ/������Ʈ�� �����մϴ�";
                UpdateAgent();
            }
            else
            {
                lbMessage.Text = "������Ʈ�� �ֽ� �����Դϴ�";
                Process.Start(path, "C.ORDER");
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
