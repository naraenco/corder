using agentcs.util;
using System;
using System.DirectoryServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using System.Windows.Forms;

namespace agentcs
{
    public partial class MainForm : Form
    {
        private Network client;
        private string shop_no = "";
        private string auth_key = "C.ORDER";
        private string path_status = "";
        private string pos_extra = "";
        private string path_order = "";
        private string print_port = "COM6";
        private int print_font_width = 0;
        private int print_font_height = 0;
        readonly Config config = Config.Instance;

        public MainForm()
        {
            InitializeComponent();
            client = new Network(MessageHandler);
            InitData();
        }

        public void InitData()
        {
            if (config.Load() == false)
                return;
            path_status = config.GetString("path_status");
            pos_extra = config.GetString("pos_extra") + ".json";
            path_order = config.GetString("path_order") + "Order_";
            shop_no = config.GetString("shop_no");
            auth_key = config.GetString("auth_key");
            print_port = config.GetString("print_port");
            print_font_width = config.GetInt("print_font_width");
            print_font_height = config.GetInt("print_font_height");
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            string address = "ws://" + config.GetString("server_address") + ":19000/ws";
            Uri serverUri = new(address);

            try
            {
                await client.ConnectAsync(serverUri);

                string auth_key = "C.오더";
                string message = "{\"msgtype\":\"login\",\"shop_no\": \""
                    + shop_no + "\",\"auth_key\":\""
                    + auth_key + "\"}";
                await client.SendAsync(message);
                Console.WriteLine("Sent message: " + message);

                //await client.CloseAsync();      // 연결 종료
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private async void buttonGenPin_Click(object sender, EventArgs e)
        {
            string message = "{\"msgtype\":\"genpin\",\"shop_no\": \"" + shop_no + "\"}";
            await client.SendAsync(message);
            Console.WriteLine("Sent message: " + message);
        }

        public void MessageHandler(string message)
        {
            Console.WriteLine("MessageHandler : " + message);

            try
            {
                var json = JsonNode.Parse(message);
                json = json?.Root;
                if (json == null) return;
            
                string msgtype = json["msgtype"]!.ToString();

                switch (msgtype)
                {
                    case "login":
                        Console.WriteLine("MessageHandler.login");
                        // 로그인에 성공하면 POS 데이터 전송
                        //SendPosData();
                        break;

                    case "genpin":
                        Console.WriteLine("MessageHandler.genpin");
                        // 핀 생성 성공하면 감열식 프린터로 인쇄
                        //string pin = "";
                        //string createdAt = "";
                        //ThemalPrint print = new();
                        //print.PrintPin(print_port, 
                        //    pin, 
                        //    createdAt, 
                        //    print_font_width, 
                        //    print_font_height);
                        break;

                    case "order":
                        Console.WriteLine("MessageHandler.order");
                        Order(json);
                        break;

                    case "mylist":
                        Console.WriteLine("MessageHandler.mylist");
                        break;

                    case "tablemap":
                        Console.WriteLine("MessageHandler.tablemap");
                        break;

                    case "menu":
                        Console.WriteLine("MessageHandler.menu");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MessageHandler : {0}", ex.Message);
            }
        }

        public async void SendPosData()
        {
            JsonWrapper jsonTableMap = new();
            if (jsonTableMap.Load(config.GetString("path_tablemap"), codepage: 51949) == true)
            {
                jsonTableMap.SetOptions(false);
                jsonTableMap.Parse();
            }

            JsonWrapper jsonMenu = new();
            if (jsonMenu.Load(config.GetString("path_menu"), codepage: 51949) == true)
            {
                jsonMenu.SetOptions(false);
                jsonMenu.Parse();
            }

            string message = "{\"msgtype\":\"tablemap\",\"shop_no\": \"" 
                + shop_no 
                + "\", \"data\":"
                + jsonTableMap.ToString()
                + "}";
            await client.SendAsync(message);

            message = "{\"msgtype\":\"menu\",\"shop_no\": \""
                + shop_no
                + "\", \"data\":"
                + jsonTableMap.ToString()
                + "}";
            await client.SendAsync(message);
        }

        public void Order(JsonNode node)
        {
            Console.WriteLine("Order()");

            try
            {
                string regdate = node["regdate"]!.ToString();
                string path = path_order + regdate + pos_extra;
                JsonNode orderList = node["pos_order"]!["orderList"]!;

                node["status"] = 1;
                Console.WriteLine(node.ToJsonString());

                JsonObject obj = new();
                obj.Add("tableNo", node["table_cd"]?.ToString());
                obj.Add("orderList", JsonNode.Parse(orderList.ToJsonString()));

                JsonUtil.WriteFile(path, obj, indent: true, codepage: 51949);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
