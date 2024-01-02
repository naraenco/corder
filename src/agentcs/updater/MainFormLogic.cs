using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace COrderUpdater
{
    partial class MainForm
    {

        public static async Task<string?> CallApiAsync(string url)
        {
            using HttpClient client = new();
            try
            {
                var buffer = await client.GetByteArrayAsync(url);
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                return responseString;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"CallApiAsync Exception : {e.Message}");
                return null;
            }
        }

        public async void UpdateAgent()
        {
            Process[] processes = Process.GetProcessesByName("corderagent");
            if (processes.Length != 0)
            {
                lbMessage.Text = "프로세스가 실행중입니다. 종료하고 업데이트를 시작합니다";
                for(int i=0; i<processes.Length; i++)
                {
                    Process process = processes[i];
                    processes[i].Kill();
                }
            }
            await DownloadAgent(FULLPACK);
            Close();
        }

        public static async Task DownloadAgent(String filename)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var result = await httpClient.GetAsync(update_url + filename);
                    result.EnsureSuccessStatusCode();

                    //MessageBox.Show(result.ToString());

                    using (var stream = await result.Content.ReadAsStreamAsync())
                    {
                        using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            await stream.CopyToAsync(fs);
                        }
                    }

                    if (File.Exists(filename))
                    {
                        Process.Start(filename, "");
                        //await Process.Start(filename, "").WaitForExitAsync();
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                MessageBox.Show("다운로드에 실패했습니다");
                Console.WriteLine($"{nameof(HttpRequestException)} - {hre.Message}");
            }
            catch (Exception e)
            {
                MessageBox.Show("다운로드에 실패했습니다");
                Console.WriteLine($"Error : {e.Message}{Environment.NewLine} Data : {e.Data}{Environment.NewLine} StackTrace : {e.StackTrace}");
            }
        }


    }
}
