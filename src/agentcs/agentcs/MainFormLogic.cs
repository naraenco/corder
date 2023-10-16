using System.Text.Json.Nodes;
using System.Net.WebSockets;
using Serilog;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Cryptography;

namespace agentcs
{
    partial class MainForm
    {
        public static async Task<string?> CallApiAsync(string apiUrl)
        {
            using HttpClient client = new();
            try
            {
                var buffer = await client.GetByteArrayAsync(apiUrl);
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                return responseString;
            }
            catch (HttpRequestException e)
            {
                Log.Error($"CallApiAsync Exception : {e.Message}");
                return null;
            }
        }

        public string ComputeMD5(string s)
        {
            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s)))
                            .Replace("-", "");
            }
        }

        public void StatusHandler(WebSocketError error)
        {
            switch (error)
            {
                case WebSocketError.Success:
                    client.connect_status = true;
                    Log.Information("StatusHandler : CONNECTED");
                    break;

                case WebSocketError.Faulted:
                    client.connect_status = false;
                    Log.Information("StatusHandler : FAULTED");
                    break;

                //case WebSocketError.InvalidState:
                //    break;

                default:
                    client.connect_status = false;
                    Log.Information("StatusHandler : DISCONNECTED");
                    break;
            }
        }

        public void MessageHandler(string message)
        {
            Log.Debug("MessageHandler : " + message);

            try
            {
                var json = JsonNode.Parse(message);
                json = json?.Root;
                if (json == null) return;

                string msgtype = json["msgtype"]!.ToString();

                switch (msgtype)
                {
                    case "login":
                        Log.Verbose("MessageHandler.login");
                        LoginAns(json);
                        break;

                    case "genpin":
                        Log.Verbose("MessageHandler.genpin");
                        if (print_use != false) GenPinAns(json);
                        break;

                    case "order":
                        Log.Verbose("MessageHandler.order");
                        OrderAns(json);
                        break;

                    case "mylist":
                        Log.Verbose("MessageHandler.mylist");
                        break;

                    default:
                        Log.Warning("Unknown Message Type");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("MessageHandler : {0}", ex.Message);
            }
        }

        public async void SendPosData()
        {
            Log.Information("SendPosData()");

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

            JsonWrapper jsonUseTable = new();
            if (jsonUseTable.Load(config.GetString("path_usetable"), codepage: 51949) == true)
            {
                jsonUseTable.SetOptions(false);
                jsonUseTable.Parse();
            }

            JsonWrapper jsonMenu = new();
            if (jsonMenu.Load(config.GetString("path_menu"), codepage: 51949) == true)
            {
                jsonMenu.SetOptions(false);
                jsonMenu.Parse();

                JsonArray products = jsonMenu.GetNode("PRODUCT").AsArray();
                foreach (var product in products)
                {
                    string key = product!["PROD_CD"]!.ToString();
                    string name = product!["PROD_NM"]!.ToString();
                    string price = product!["SALE_UPRC"]!.ToString();

                    dicMenuName.Add(key, name);
                    dicMenuPrice.Add(key, price);
                }
            }

            JsonWrapper jsonTouchClass = new();
            if (jsonTouchClass.Load(config.GetString("path_touchclass"), codepage: 51949) == true)
            {
                jsonTouchClass.SetOptions(true);
                jsonTouchClass.Parse();
            }

            JsonWrapper jsonTouchKey = new();
            if (jsonTouchKey.Load(config.GetString("path_touchkey"), codepage: 51949) == true)
            {
                jsonTouchKey.SetOptions(true);
                jsonTouchKey.Parse();
            }

            string message = "{\"msgtype\":\"tablemap\",\"shop_no\": \""
                + shop_no
                + "\", \"scdtable\":"
                + jsonScdTable.ToString()
                + ", \"usetable\":"
                + jsonUseTable.ToString()
                + "}";
            await client.SendAsync(message);

            string url1 = apiUrl + "sync_tables.php?shop_no=" + shop_no;
            try
            {
                string? response = await CallApiAsync(url1);
                Log.Debug("sync_tables response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"sync_tables exception : {ex.Message}");
            }

            message = "{\"msgtype\":\"menu\",\"shop_no\": \""
                + shop_no
                + "\", \"touch_class\":"
                + jsonTouchClass.ToString()
                + ", \"touch_key\":"
                + jsonTouchKey.ToString()
                + ", \"data\":"
                + jsonMenu.ToString()
                + "}";
            await client.SendAsync(message);

            string url2 = apiUrl + "sync_menus.php?shop_no=" + shop_no;
            try
            {
                string? response = await CallApiAsync(url2);
                Log.Debug("sync_menus response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"sync_menus exception : {ex.Message}");
            }
        }

        public async void Connect()
        {
            string address = "ws://" + config.GetString("server_address")
                + ":" + config.GetString("server_port") + "/ws";
            Uri serverUri = new(address);
            Log.Information("Server Address : " + address);
            await client.ConnectAsync(serverUri);
        }

        public async void LoginReq(string business_number, string password)
        {
            if (client.connect_status == false)
            {
                MessageBox.Show(this, "서버와 연결이 되어있지 않습니다");
                return;
            }

            Log.Information("LoginReq()");
            string login_pass = ComputeMD5(password);

            try
            {
                string message = "{\"msgtype\":\"login\",\"shop_no\": \"" + shop_no 
                    + "\",\"business_number\":\"" + business_number
                    + "\",\"login_pass\":\"" + login_pass
                    + "\"}";
                await client.SendAsync(message);
                Log.Debug("LoginReq : " + message);
            }
            catch (Exception ex)
            {
                Log.Error("LoginReq : {0}", ex.Message);
            }
        }

        public async void GenPinReq()
        {
            string message = "{\"msgtype\":\"genpin\",\"shop_no\": \"" + shop_no + "\"}";
            await client.SendAsync(message);
            Log.Debug("GenPinReq : " + message);
        }

        public void GenPinAns(JsonNode node)
        {
            Log.Information("GenPinAns()");

            try
            {
                string pin = node["pin"]!.ToString();
                string createdAt = node["created_at"]!.ToString();
                if (String.IsNullOrEmpty(pin))
                {
                    Log.Warning("pin is null or empty");
                    return;
                }
                //ThemalPrint print = new();
                themalPrint.PrintPin(pin, createdAt, print_pin_width, print_pin_height);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        
        public void LoginAns(JsonNode node)
        {
            try
            {
                string result = node["result"]!.ToString();
                Log.Debug("LoginAns : " + result);

                if (result == "false")
                {
                    formLogin?.ResultMessage("로그인에 실패했습니다");
                    return;
                }

                formLogin?.LoginSuccess();

                SendPosData();
                SendTableStatus();
                StatusMonitor();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

        }

        public void OrderAns(JsonNode node)
        {
            Log.Information("OrderAns()");

            try
            {
                string orderno = node["order_no"]!.ToString();
                string regdate = node["regdate"]!.ToString();
                string path = path_order + regdate + pos_extra + orderno + ".json";
                JsonNode orderList = node["pos_order"]!["orderList"]!;

                CultureInfo provider = CultureInfo.InvariantCulture;
                //DateTime dt = DateTime.ParseExact(regdate, "yyyy:MM:dd HH:mm:ss", provider);
                DateTime dt = DateTime.ParseExact(regdate, "yyyyMMddHHmmss", null);

                node["status"] = 1;

                JsonObject obj = new();
                string tableNo = node["table_cd"]?.ToString()!;

                obj.Add("tableNo", tableNo);
                obj.Add("orderList", JsonNode.Parse(orderList.ToJsonString()));
                JsonUtil.WriteFile(path, obj, indent: true, codepage: 51949);
                Log.Debug(path);


                if (print_use != false)
                {
                    string orderText = "[모바일 오더 주문서]\n\n";

                    orderText += "[테 이 블] " + tableNo + "\n";
                    orderText += "[발행일시] " + dt.ToString() + "\n";
                    orderText += "==========================================\n";
                    orderText += "  메뉴명                            수량\n";
                    orderText += "------------------------------------------\n";

                    foreach (var order in orderList.AsArray())
                    {
                        string productCode = order!["productCode"]!.ToString();
                        string qty = order!["qty"]!.ToString();
                        string productName = dicMenuName[productCode];

                        byte[] data = Encoding.Unicode.GetBytes(productName);
                        int count = data.Length;
                        int pad = 38 - count;
                        orderText += productName + qty.PadLeft(pad) + "\n";
                    }
                    orderText += "------------------------------------------\n\n";

                    Console.WriteLine(orderText);

                    //ThemalPrint print = new();
                    themalPrint.PrintOrder(orderText);
                }

                if (order_popup != false)
                {
                    List<string> productList = new();
                    List<string> qtyList = new();

                    foreach (var order in orderList.AsArray())
                    {
                        string productCode = order!["productCode"]!.ToString();
                        string qty = order!["qty"]!.ToString();
                        string productName = dicMenuName[productCode];

                        productList.Add(productName);
                        qtyList.Add(qty);
                    }

                    //Thread dialogThread = new Thread(() => CreateOrderDialog(tableNo,
                    //    dt.ToString(),
                    //    productList,
                    //    qtyList));
                    //dialogThread.Start();

                    this.formOrder!.AddData(tableNo, dt.ToString(), productList, qtyList);
                    if (this.formOrder.Visible == false)
                    {
                        this.formOrder.SetData();
                    }
                    this.formOrder.Show();

                    //FormOrder formOrder = new()
                    //{
                    //    TopLevel = true
                    //};
                    //Point parentPoint = this.Location;
                    //formOrder.Location = parentPoint;
                    //formOrder.Top += 54;
                    //formOrder.SetData(tableNo, dt.ToString(), productList, qtyList);
                    //formOrder.Show();
                    //formOrder.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async void SendTableStatus()
        {
            JsonWrapper jsonTableStatus = new();
            if (jsonTableStatus.Load(path_status, codepage: 51949) == true)
            {
                jsonTableStatus.SetOptions(false);
                jsonTableStatus.Parse();
            }

            if (!String.IsNullOrEmpty(lastTableStatus?.ToString()) && jsonTableStatus.ToString() == lastTableStatus?.ToString())
            {
                return;
            }

            Log.Information("SendTableStatus()");

            try
            {
                List<string> orders = new();

                JsonArray currTables = jsonTableStatus.GetNode("tableList").AsArray();
                foreach (var table in currTables)
                {
                    var obj = table?.AsObject();
                    if (obj?.ContainsKey("orderList") == true)
                    {
                        string tableNo = table?["tableNo"]!.ToString()!;
                        orders?.Add(tableNo);
                    }
                }

                if (lastTableStatus != null)
                {
                    JsonArray lastTables = lastTableStatus.GetNode("tableList").AsArray();
                    foreach (var table in lastTables)
                    {
                        var obj = table?.AsObject();
                        if (obj?.ContainsKey("orderList") == true)
                        {
                            string tableNo = table?["tableNo"]!.ToString()!;
                            if (orders.Contains(tableNo) == false)
                            {
                                SendClear(1, tableNo);
                            }
                        }
                    }
                }

                lastTableStatus = new();
                lastTableStatus.Parse(jsonTableStatus.ToString()!);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            string message = "{\"msgtype\":\"tablestatus\",\"shop_no\": \""
                    + shop_no
                    + "\", \"data\":"
                    + jsonTableStatus.ToString()
                    + "}";
            await client.SendAsync(message);
        }

        public async void SendClear(int type, string table_cd)
        {
            Log.Information("SendClear() - type : {0}, table : {1}", type, table_cd);

            string message = "{\"msgtype\":\"clear\",\"shop_no\": \""
                    + shop_no
                    + "\", \"table_cd\": \""
                    + table_cd
                    + "\", \"type\": "
                    + type
                    + "}";
            Console.WriteLine(message);
            await client.SendAsync(message);

            string url = apiUrl + $"delete_connect.php?shop_no={shop_no}&table_cd={table_cd}";
            try
            {
                string? response = await CallApiAsync(url);
                Log.Debug("delete_connect response : " + response);
            }
            catch (Exception ex)
            {
                Log.Error($"delete_connect exception : {ex.Message}");
            }
        }

        public string GetTableCodeByName(string tableName)
        {
            string table_cd = dicScdTable[tableName];
            return table_cd;
        }

        public string GetTableNameByCode(string code)
        {
            string table_nm = string.Empty;
            foreach (string key in dicScdTable.Keys)
            {
                if (dicScdTable[key] == code)
                {
                    table_nm = key;
                    break;
                }
            }
            return table_nm;
        }

        public void CreateOrderDialog(string tableName,
            string datetime,
            List<string> productList,
            List<string> qtyList)
        {
            FormOrder formOrder = new()
            {
                TopLevel = true
            };
            Point parentPoint = this.Location;
            formOrder.Location = parentPoint;
            formOrder.Top += 54;
            formOrder.AddData(tableName, datetime, productList, qtyList);
            formOrder.ShowDialog();
        }
    }
}
