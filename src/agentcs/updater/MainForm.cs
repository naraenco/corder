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
            lbMessage.Text = "버전을 확인중입니다";

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
                lbMessage.Text = "서버가 응답하지 않습니다.";
                MessageBox.Show("서버가 응답하지 않습니다.");
                return;
            }

            JsonNode? node = JsonNode.Parse(response!);
            if (node == null)
            {
                lbMessage.Text = "서버에서 버전 정보를 얻을 수 없습니다.";
                MessageBox.Show("서버에서 버전 정보를 얻을 수 없습니다.");
                return;
            }
            current_version = node["version"]!.ToString();
            lbCurrentVersion.Text = current_version;
            lbFileVersion.Text = "";
            bool bUpdate = false;


            if (File.Exists(path) == false)
            {
                lbFileVersion.Text = "";
                lbMessage.Text = "에이전트 파일이 없습니다. 신규 설치를 진행합니다";
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
                    lbMessage.Text = "버전을 확인할 수 없습니다";
                    return;
                }
            }

            if (bUpdate == true)
            {
                lbMessage.Text = "설치/업데이트를 진행합니다";
                UpdateAgent();
            }
            else
            {
                lbMessage.Text = "에이전트가 최신 버전입니다";
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
