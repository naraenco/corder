using System.Text.Json.Nodes;


namespace agentcs
{
    public sealed class Config
    {
        private JsonNode? root = null;

        private Config()
        {
        }

        private static readonly Lazy<Config> _instance = new Lazy<Config>(() => new Config());
        public static Config Instance { get { return _instance.Value; } }

        public void Load()
        {
            string json = File.ReadAllText("config.json");
            JsonNode document = JsonNode.Parse(json)!;
            root = document.Root;
            //Console.WriteLine(root);
        }

        public string GetString(string key)
        {
            string result = "";
            if (root != null)
            {
                result = root[key]!.ToString();
            }
            return result;
        }

        public int GetInt(string key)
        {
            int result = -1;
            if (root != null)
            {
                result = (int)root[key]!;
            }
            return result;
        }
    }
}
