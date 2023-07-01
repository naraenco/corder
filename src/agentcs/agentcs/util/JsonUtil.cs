using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;


namespace agentcs
{
    public class JsonUtil
    {
        static public bool WriteFile(string path, JsonObject obj, bool indent = true, int codepage = 0)
        {
            if (obj == null) return false;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            TextEncoderSettings encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowRange(UnicodeRanges.All);
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(encoderSettings)
            };

            string content = obj.ToJsonString(options);

            if (codepage == 0)
            {
                File.WriteAllText(path, content);
            }
            else
            {
                Encoding encode = Encoding.GetEncoding(codepage);
                File.WriteAllText(path, content, encode);
            }
            return true;
        }
    }
}
