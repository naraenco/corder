namespace agentcs
{
    public partial class MainForm : Form
    {
        private Network client;
        private string? shop_no;
        private string? auth_key;
        private string? path_status;
        private string? pos_extra;
        private string? path_order;
        private string? print_port;
        private int? print_font_width;
        private int? print_font_height;
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
            shop_no = config.GetString("shop_no");
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
            PosData.LoadJsonFiles();
            //PosData pos = new();
            //pos.LoadJsonFiles();

            string address = "ws://" + config.GetString("server_address") + ":19000/ws";
            Uri serverUri = new(address);

            try
            {
                await client.ConnectAsync(serverUri);

                string shop_no = "3062";
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
            string shop_no = "3062";
            string message = "{\"msgtype\":\"genpin\",\"shop_no\": \"" + shop_no + "\"}";
            await client.SendAsync(message);
            Console.WriteLine("Sent message: " + message);
        }

        public void MessageHandler(string message)
        {
            Console.WriteLine("Received message: " + message);

            JsonWrapper json = new();
            json.Parse(message);
            string msgtype = json.GetString("msgtype");

            switch (msgtype)
            {
                case "login":
                    Console.WriteLine("MessageHandler.login");
                    break;

                case "genpin":
                    Console.WriteLine("MessageHandler.genpin");
                    break;

                case "order":
                    Console.WriteLine("MessageHandler.order");
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
    }
}
