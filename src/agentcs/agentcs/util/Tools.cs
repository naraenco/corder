using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agentcs.util
{
    public class Tools
    {
        public static void Browse(string url)
        {
            //string url = "http://corder.co.kr/manager";
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

