using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.Tool
{
    public class ProcessTool
    {
        /// <summary>
        /// 执行指定字符串命令
        /// </summary>
        /// <param name="cmd">命令字符串</param>
        /// <returns></returns>
        public static string ExecuteCmd(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.Start();
            process.StandardInput.WriteLine(cmd + "&exit");
            process.StandardInput.AutoFlush = true;
            string output = process.StandardOutput.ReadToEnd();
            Trace.TraceInformation(output);

            string error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                Trace.TraceError(error);
            }
            process.WaitForExit();
            process.Close();
            return output + error;
        }
    }
}
