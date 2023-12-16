using System.Diagnostics;
using System.Text.Json.Nodes;

namespace COrderUpdater
{
    public partial class MainForm : Form
    {
        readonly static string APPNAME = "COrderAgent.exe";
        readonly static string PACKNAME = "COrderPack.exe";

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
            lbMessage.Text = "버전을 확인중입니다";

            if (config.Load() == false)
                return;

            server_address = config.GetString("server_address");
            api_url = "http://" + server_address + ":" + server_port + "/agent-version";

            CheckVersion();
        }

        public async void CheckVersion()
        {
            string? response = await CallApiAsync(api_url);
            JsonNode? node = JsonNode.Parse(response!);
            if (node == null)
            {
                lbMessage.Text = "서버에서 버전 정보를 얻을 수 없습니다.";
                return;
            }
            current_version = node["version"]!.ToString();
            lbCurrentVersion.Text = current_version;
            //String status = DiffVersion();

            lbFileVersion.Text = "";
            path = Directory.GetCurrentDirectory() + "\\" + APPNAME;
            if (File.Exists(path) == false)
            {
                // 신규 다운로드 
                lbFileVersion.Text = "";
                lbMessage.Text = "에이전트 파일이 없습니다. 신규 설치를 진행합니다";
                await DownloadAgent(PACKNAME);
                lbMessage.Text = "다운로드 완료";
                path = Directory.GetCurrentDirectory() + "\\" + PACKNAME;
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
                    lbMessage.Text = "업데이트를 진행합니다";
                    path = Directory.GetCurrentDirectory() + "\\" + APPNAME;
                    UpdateAgent();
                    return;
                }
                else
                {
                    lbMessage.Text = "에이전트가 최신 버전입니다";
                    path = Directory.GetCurrentDirectory() + "\\" + APPNAME;
                    Process.Start(path, "");
                    Close();
                    return;
                }
            }
            else
            {
                lbMessage.Text = "버전을 확인할 수 없습니다";
            }
            //Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
