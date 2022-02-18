using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs
{
    public static class FFprobeHelper
    {
        public static ProcessStartInfo psi = new ProcessStartInfo();

        static FFprobeHelper()
        {
            psi.FileName = "ffprobe.exe";
            psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
        }

        public static async Task<string> GetMediaInfo(string source)
        {
            string jsonOutput;

            Process process = new Process();
            process.StartInfo = psi;
            process.StartInfo.Arguments = "-i \"" + source + "\" -v quiet -print_format json -show_format -show_streams -hide_banner";
            process.Start();

            jsonOutput = await Task.Run(() =>
            {
                StringBuilder stdoutBuilder = new StringBuilder();

                while (process.StandardOutput.Peek() > -1)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.Contains("probe_score")) //由于某些原因，ffprobe有时会在关闭前挂起30-60秒，因此有必要在输出结束时手动停止
                    {
                        break;
                    }
                    stdoutBuilder.Append(line);
                }
                stdoutBuilder.Remove(stdoutBuilder.Length - 1, 1); //删除最后一个逗号
                return stdoutBuilder.ToString();
            }).ConfigureAwait(false);

            if (jsonOutput.Length < 5)
            {
                //由于无法从此线程创建窗口，因此会引发异常；调用此方法的人必须向用户显示错误
                throw new Exception("Failed to parse media file:\n\n" + source);
            }

            //由于某些原因，输出制动器有时不平衡，因此有必要手动平衡它们
            while (!BracketBalanced(jsonOutput)) jsonOutput += "}";

            return jsonOutput;
        }

        public static async Task<string> GetColorInfo(string source)
        {
            Process process = new Process();
            process.StartInfo = psi;
            process.StartInfo.Arguments = "-i \"" + source + "\" -v quiet -print_format json -select_streams v -show_frames -read_intervals \"%+#1\" -show_entries \"frame=color_space,color_primaries,color_transfer,side_data_list,pix_fmt\" -hide_banner";
            process.Start();

            return await Task.Run(() =>
            {
                StringBuilder stdoutBuilder = new StringBuilder();

                while (process.StandardOutput.Peek() > -1)
                {
                    string line = process.StandardOutput.ReadLine();
                    stdoutBuilder.Append(line);
                    if (line.Contains("probe_score")) //由于某些原因，ffprobe有时会在关闭前挂起30-60秒，因此有必要在输出结束时手动停止
                    {
                        break;
                    }
                }

                return stdoutBuilder.ToString();
            }).ConfigureAwait(false);
        }

        public static async Task<(double before, double after, bool isKeyFrame)> GetNearestBeforeAndAfterKeyFrames(MediaInfo mediaInfo, double position)
        {
            Process process = new Process();
            string line;
            double nearestKeyFrameBefore = 0;
            double nearestKeyFrameAfter = mediaInfo.Duration.TotalSeconds;
            bool isKeyFrame = position == 0;
            const double MAX_DISTANCE = 30;
            double distance = 2;
            double increment = 2;
            double startSeekPosition, endSeekPosition;
            bool foundBefore = position == 0;
            bool foundAfter = false;

            process.StartInfo = psi;
            position = Math.Round(position, 2);

            do
            {
                distance += increment++;
                startSeekPosition = position > distance ? position - distance : 0;
                endSeekPosition = position + distance;
                process.StartInfo.Arguments = $"-read_intervals {startSeekPosition:0.00}%{endSeekPosition:0.00} -select_streams v -skip_frame nokey -show_entries frame=key_frame,pkt_pts_time -print_format csv=p=0 \"{mediaInfo.Source}\" -hide_banner";
                process.Start();

                await Task.Run(() =>
                {
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        string[] values = line.Split(','); // //对于vp9 videos -skip-frame nokey 不起作用，因此有必要包括并检查key_frame的标志(flag)
                        if (values[0] == "1")
                        {
                            double currentPosition = Ceiling(Double.Parse(values[1]), 2);
                            if (currentPosition < position)
                            {
                                if (currentPosition > nearestKeyFrameBefore)
                                {
                                    nearestKeyFrameBefore = currentPosition;
                                    foundBefore = true;
                                }
                            }
                            else if (currentPosition > position)
                            {
                                if (currentPosition < nearestKeyFrameAfter)
                                {
                                    nearestKeyFrameAfter = currentPosition;
                                    foundAfter = true;
                                }
                            }
                            else
                            {
                                isKeyFrame = true;
                            }
                        }
                    }
                }).ConfigureAwait(false);
            } while ((!foundBefore || !foundAfter) && distance < MAX_DISTANCE);

            return (nearestKeyFrameBefore, nearestKeyFrameAfter, position > 0 ? isKeyFrame : true);
        }

        private static bool BracketBalanced(string text)
        {
            int brackets = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '{') brackets++;
                else if (text[i] == '}') brackets--;
            }
            return brackets == 0;
        }

        private static double Ceiling(double value, int digits)
        {
            // With digits = 2:
            // 5.123456 -> 5.13
            // 5.001000 -> 5.01
            // 5.110999 -> 5.12
            int x = (int)(value * Math.Pow(10, digits));
            x += 1;
            return x / Math.Pow(10, digits);
        }
    }
}
