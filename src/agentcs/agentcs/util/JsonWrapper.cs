using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace agentcs
{
    public class JsonWrapper
    {
        private JsonNode? root = null;
        private string? str = null;
        private JsonSerializerOptions options;
        private TextEncoderSettings encoderSettings;

        public JsonWrapper()
        {
            encoderSettings = new();
            encoderSettings.AllowRange(UnicodeRanges.All);

            options = new()
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(encoderSettings)
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void SetOptions(bool flag)
        {
            options.WriteIndented = flag;
        }

        public bool Load(string path, int codepage = 0)
        {
            if (!File.Exists(path)) return false;

            try
            {
                if (codepage == 0)
                {
                    str = File.ReadAllText(path);
                }
                else
                {
                    Encoding encode = Encoding.GetEncoding(codepage);
                    var bytes = File.ReadAllBytes(path);
                    str = encode.GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("JsonWrapper.Load : {0}", ex.Message);
                return false;
            }
            return true;
        }

        public bool Parse()
        {
            if (str == null) return false;

            try
            {
                var data = JsonNode.Parse(str);
                root = data?.Root;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("JsonWrapper.Parse : {0}", ex.Message);
            }
            return false;
        }

        public bool Parse(string data)
        {
            if (String.IsNullOrEmpty(data)) return false;
            str = data;
            return Parse();
        }

        public bool Save(string path, bool indent = true, int codepage = 0)
        {
            if (root == null) return false;
            JsonSerializerOptions saveOptions = new()
            {
                WriteIndented = indent,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(encoderSettings)
            };

            string data = root.ToJsonString(saveOptions);

            if (codepage == 0)
            {
                File.WriteAllText(path, data);
            }
            else
            {
                Encoding encode = Encoding.GetEncoding(codepage);
                File.WriteAllText(path, data, encode);
            }
            return true;
        }

        public JsonNode Get()
        {
            return root!;
        }

        public string GetString(string key)
        {
            if (root == null) return "";
            return root[key]!.ToString();
        }

        public int GetInt(string key)
        {
            if (root == null) return -1;
            return (int)root[key]!;
        }

        public JsonNode GetNode(string key)
        {
            return root![key]!;
        }

        public override string? ToString()
        {
            if (root == null) return "";
            return root.ToJsonString(options);
        }
    }
}
