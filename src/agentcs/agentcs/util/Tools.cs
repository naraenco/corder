using System.Diagnostics;

namespace agentcs.util
{
    public class Tools
    {
        public static void Browse(string url)
        {
            string command = $"start {url}";
            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = string.Format("/C {0}", command);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }
    }
}

