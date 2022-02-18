using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs
{
    /// <summary>
    /// 寻找ffmpeg的路径,并把路径告诉AutoGen
    /// </summary>
    public class FFmpegBinariesHelper
    {
        internal static void RegisterFFmpegBinaries()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var current = Environment.CurrentDirectory;
                //var probe = Path.Combine("FFmpeg", "bin", Environment.Is64BitProcess ? "x64" : "x86");
                while (current != null)
                {
                    var ffmpegBinaryPath = current;//Path.Combine(current, probe);

                    if (Directory.Exists(ffmpegBinaryPath))
                    {
                        Console.WriteLine($"FFmpeg binaries found in: {ffmpegBinaryPath}");
                        ffmpeg.RootPath = ffmpegBinaryPath;
                        return;
                    }

                    current = Directory.GetParent(current)?.FullName;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                ffmpeg.RootPath = "/lib/x86_64-linux-gnu/";
            else
                throw new NotSupportedException(); // fell free add support for platform of your choose
        }
    }
}
