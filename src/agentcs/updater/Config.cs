using System.Text.Json.Nodes;

namespace COrderUpdater
{
    public sealed class Config
    {
        private JsonNode? root = null;
        private String filename = "config.json";

        private Config()
        {
        }

        private static readonly Lazy<Config> _instance = new Lazy<Config>(() => new Config());
        public static Config Instance { get { return _instance.Value; } }

        public bool Load()
        {
            if (!File.Exists(filename)) return false;

            try
            {
                string json = File.ReadAllText(filename);
                JsonNode document = JsonNode.Parse(json)!;
                root = document.Root;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public string GetString(string key)
        {
            string result = String.Empty;
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
