using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace agentcs
{
    internal class JsonWrapper
    {
        private JsonNode? root = null;
        private string? json = null;
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
                    json = File.ReadAllText(path);
                }
                else
                {
                    Encoding encode = Encoding.GetEncoding(codepage);
                    var bytes = File.ReadAllBytes(path);
                    json = encode.GetString(bytes);
                }
                //Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool Parse()
        {
            if (json == null) return false;

            try
            {
                var document = JsonNode.Parse(json);
                root = document?.Root;
                //Console.WriteLine(root.ToJsonString(options));
                //Console.WriteLine(root["PRODUCT"][0]["PROD_NM"]);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool Parse(string data)
        {
            if (String.IsNullOrEmpty(data)) return false;
            json = data;
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

        public override string? ToString()
        {
            if (root == null) return "";
            return root.ToJsonString(options);
        }
    }
}
