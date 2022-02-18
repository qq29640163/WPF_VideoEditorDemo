//using FFmpeg.AutoGen;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ZPC.Phone.FFmpegs
{
    /*
     * 时间线编辑语法 ：
     * 1.smartblur=enable='between(t,10,3*60)'表示smartblur模糊滤镜从第10秒持续到3分钟
     * 2.curves=enable(gte(t,3))表示在3秒后启用滤镜
     */
    //ffmpeg视频合成语法官方：https://trac.ffmpeg.org/wiki/Concatenate#samecodec
    //ffmpeg 过滤器语法详解1：https://www.jianshu.com/p/3c8c4a892f3c
    //ffmpeg 过滤器语法详解2：https://www.cnblogs.com/lidabo/p/15406984.html
    public static class FFmpegHelper
    {
        public delegate void OutPutFFmpegCommadExecLog(string log);

        public static event OutPutFFmpegCommadExecLog OutPutFFmpegCommadExecEvent;

        private static string output;
        private static string error;
        private static int IsFFmpeg = 0;//0 ffmpeg,1 ffprobe 2 ffplay

        /// <summary>
        /// 进度变化事件
        /// </summary>
        public static event Action<ProgressData> ProgressChanged;
        public static ProgressData progressData;

        public static string ExecFFmpegCommand(string cmd, int isffmpeg = 0)
        {
            progressData = new ProgressData();
            IsFFmpeg = isffmpeg;
            output = string.Empty;
            error = string.Empty;
            Process process = new Process();
            if (isffmpeg == 0)
                process.StartInfo.FileName = "ffmpeg.exe";
            else if(isffmpeg == 1)
                process.StartInfo.FileName = "ffprobe.exe";
            else
                process.StartInfo.FileName = "ffplay.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.StartInfo.Arguments = cmd;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();


            process.WaitForExit();
            output += "&Exists Complete!";
            process.Close();
            return output + error;
        }


        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (IsFFmpeg!=0)
                output += e.Data;
            if (!string.IsNullOrEmpty(e.Data))
            {
                OutPutFFmpegCommadExecEvent?.Invoke(e.Data);
                if (!e.Data.Contains('=')) return;
                string a = e.Data.Replace("  ", "");
                a = a.Replace("= ", "=");
                var s = a.Split(' ');
                string str= s.Where(str => 
                {
                    return str.Contains("time");
                }).FirstOrDefault();

                if (string.IsNullOrEmpty(str) || !str.Contains("time"))
                    return;
                (string key, string value) = ToCouple(str.Split('='));
                progressData.CurrentTime = TimeSpan.Parse(value);
                 ProgressChanged?.Invoke(progressData);


            }
            (string, string) ToCouple(string[] array) => (array[0], array[1]);
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (IsFFmpeg!=0)
                output += e.Data;
            if (!string.IsNullOrEmpty(e.Data))
                OutPutFFmpegCommadExecEvent?.Invoke(e.Data);
        }

        /// <summary>
        /// 执行ffmpeg命令
        /// </summary>
        /// <param name="fFmpegEnum">ffmpeg枚举</param>
        /// <param name="parameters">其他参数</param>
        /// <param name="SourcePath">源视频文件路径</param>
        /// <param name="TargetPath">目标文件保存路径</param>
        /// <returns>返回命令执行过程</returns>
        public static string ExecFFmpegCommand(FFmpegEnum fFmpegEnum, object[] parameters, string SourcePath = "", string TargetPath = "")
        {
            string strcmd = string.Empty;
            object[] para;
            switch (fFmpegEnum)
            {
                case FFmpegEnum.Get_FullDecodeTime:
                    strcmd = "-progress - -benchmark -i {0} -f null -";
                    strcmd = string.Format(strcmd, new object[] { SourcePath });
                    break;
                case FFmpegEnum.Cut_FromSecond:
                    strcmd = "-progress - -ss {0} -i \"{1}\" -c copy -copyts {2}";
                    para = new object[] { parameters[0].ToString(), SourcePath, TargetPath };
                    strcmd = string.Format(strcmd, para);
                    break;
                case FFmpegEnum.Cut_ToSecond:
                    strcmd = "-progress - -i \"{0}\" -c copy -t {1} -copyts {2}";
                    para = new object[] { SourcePath, parameters[0].ToString(), TargetPath };
                    strcmd = string.Format(strcmd, para);
                    break;
                case FFmpegEnum.Cut_FromToSecond:
                    strcmd = "-progress - -ss {0} -i \"{1}\" -t {2} -y {3}";// -loglevel quiet
                    para = new object[] { parameters[0].ToString(), SourcePath, parameters[1].ToString(), TargetPath };
                    strcmd = string.Format(strcmd, para);
                    break;
                case FFmpegEnum.Merge_Video:
                    {
                        string[] Paths = SourcePath.Split('|');
                        string sourcePath = "";
                        string concat = "";
                        for (int i = 0; i < Paths.Length; i++)
                        {
                            sourcePath += "-progress - -i \"" + Paths[i] + "\" ";
                            concat += "[" + i.ToString() + ":v][" + i.ToString() + ":a]";
                        }
                        strcmd = "{0}-filter_complex \"{1}concat=n=" + Paths.Length.ToString() + " :v=1:a=1[v][a]\" -map \"[v]\" -map \"[a]\" {2}";
                        para = new object[] { sourcePath, concat, TargetPath };
                        strcmd = string.Format(strcmd, para);
                    }
                    break;
                case FFmpegEnum.Merge_Video_FromDiffScale:
                    {
                        string[] Paths = SourcePath.Split('|');
                        string sourcePath = "";
                        string scale = "";
                        string concat = "";
                        for (int i = 0; i < Paths.Length; i++)
                        {
                            sourcePath += "-progress - -i \"" + Paths[i] + "\" ";
                            scale += "[" + i.ToString() + ":v]scale={0}[v" + i.ToString() + "];";
                            scale = string.Format(scale, parameters[0]);
                            concat += "[v" + i.ToString() + "][" + i.ToString() + ":a]";
                        }
                        strcmd = "{0}-filter_complex \"{1}{2}concat=n=" + Paths.Length.ToString() + " :v=1:a=1[v][a]\" -map \"[v]\" -map \"[a]\" -vsync 2 -y -vb {3} {4}";
                        para = new object[] { sourcePath, scale, concat, parameters[1], TargetPath };
                        strcmd = string.Format(strcmd, para);
                    }
                    break;
                case FFmpegEnum.AddImageWatermark:
                    {
                        string movie = parameters[0].ToString();
                        parameters[0] = CustomToEscape(movie);
                        //movie代表水印，scale分辨率 [in]代表输入的视频文件,wm代表水印 ,overlay覆盖滤镜
                        string filters = "movie={0},scale={1}x{2}[wm];[in][wm] overlay={3}:{4}[out]";
                        filters = string.Format(filters, parameters);
                        strcmd = "-progress - -i \"{0}\" -vf \"{1}\" -y {2}";
                        para = new object[] { SourcePath, filters, TargetPath };
                        strcmd = string.Format(strcmd, para);
                    }
                    break;
                case FFmpegEnum.AddTextWatermark:
                    {
                        string filters = "drawtext=fontsize={0}:fontfile={1}:text='{2}':fontcolor={3}:x={4}:y={5}";
                        filters = string.Format(filters, parameters);
                        strcmd = "-progress - -i \"{0}\" -vf \"{1}\" {2}";
                        para = new object[] { SourcePath, filters, TargetPath };
                        strcmd = string.Format(strcmd, para);
                    }
                    break;
                case FFmpegEnum.ConvertGif:
                    {
                        string str = "-ss {0} -to {1} -s {2} -r {3}";
                        str = string.Format(str, parameters);
                        strcmd = "-progress - -i \"{0}\" {1} {2}";
                        para = new object[] { SourcePath, str, TargetPath };
                        strcmd = string.Format(strcmd, para);
                    }
                    break;
                case FFmpegEnum.BlurDesignatedArea:
                    string strFilter = "[0:v]crop={0}:{1}:{2}:{3},{4}={5}[fg];[0:v][fg]overlay={2}:{3}[v]";
                    strFilter = string.Format(strFilter, parameters);
                    strcmd = "-progress - -i \"{0}\" -filter_complex \"{1}\" -map \"[v]\" -y {2}";
                    strcmd = string.Format(strcmd, new object[] { SourcePath, strFilter, TargetPath });
                    break;
                default:
                    break;
                    //ffmpeg -i "E:\LOL.mp4" -i "E:\QQ.png" -i "E:\weixin.png" -filter_complex "[1:v]scale=50x50[v1];[2:v]scale=50x50[v2];[0:v][v1]overlay=10:10:enable='gte(t,20)'[a];[a][v2]overlay=60:10" -y E:\outVideo.mp4
                    //ffplay "E:\LOL.mp4" -vf "drawtext=fontsize=100:fontfile=FreeSerif.ttf:text='Hello World':x=30:y=30[text];movie='E\:\\weixin.png',scale=50x50[wm];[text][wm]overlay=100:30"
                    //因为ffplay不支持-filter_complex,所有用:ffplay -f lavfi "movie='E\:\\LOL.mp4'[aaa];[aaa]drawtext=fontsize=100:fontfile=FreeSerif.ttf:text='Hello World':x=30:y=30[text];movie='E\:\\weixin.png',scale=50x50[wm];[text][wm]overlay=100:30"
            }
            strcmd = strcmd + " -loglevel error -stats";
            return ExecFFmpegCommand(strcmd);
        }

        public static void ExecFFplayCommand(string SourcePath,string filter)
        {
            string cmd = SourcePath + " " + filter;
            ExecFFmpegCommand(cmd,2);
        }

        /// <summary>
        /// 执行滤镜滤镜方法
        /// </summary>
        /// <param name="isComplexFilter">是否为复杂滤镜，true是-filter_complex复杂滤镜,false是-vf简单滤镜</param>
        /// <param name="SourcePath">输入的音视频路径</param>
        /// <param name="TargetPath">输出的音视频路径</param>
        /// <param name="filter">拼接好的滤镜字符串</param>
        /// <returns></returns>
        public static string ExecFilter(bool isComplexFilter,string SourcePath,string TargetPath,string filter)
        {
            string cmd;
            if (isComplexFilter)
                cmd = $"-progress - -i \"{SourcePath}\" -filter_complex \"{filter}\" -y \"{TargetPath}\"";
            else
                cmd = $"-progress - -i \"{SourcePath}\" -vf \"{filter}\" -y \"{TargetPath}\"";
            cmd = cmd + " -loglevel error -stats";
            return ExecFFmpegCommand(cmd);
        }

        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string CustomToEscape(string str)
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\\': // 反斜杠
                        sb.Append("\\\\\\\\");
                        break;
                    case ':':
                        sb.Append("\\\\:");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 执行ffprobe命令,详细教程//https://trac.ffmpeg.org/wiki/FFprobeTips
        /// </summary>
        /// <param name="fFplayEnum">ffprobe命令枚举</param>
        /// <param name="parameters">参数数组</param>
        /// <param name="SourcePath">源视频文件路径</param>
        /// <param name="TargetPath">目标文件保存路径</param>
        /// <returns>返回命令执行过程</returns>
        public static string ExecFFprobeCommand(FFprobeEnum fFprobe, object[] parameters, string SourcePath, string TargetPath)
        {
            string strcmd = string.Empty;
            switch (fFprobe)
            {
                case FFprobeEnum.GetVideoAllInfo:
                    strcmd = "-v error -show_format -show_streams \"{0}\"";
                    strcmd = string.Format(strcmd, SourcePath);
                    break;
                case FFprobeEnum.GetVideoResolution:
                    strcmd = "-v error -select_streams v:0 -show_entries stream=height,width -of csv=s=x:p=0 \"{0}\"";
                    strcmd = string.Format(strcmd, SourcePath);
                    break;
                case FFprobeEnum.GetVideoBitrate:
                    strcmd = "-v quiet -print_format json -show_format -i \"{0}\"";
                    strcmd = string.Format(strcmd, SourcePath);
                    break;
                default:
                    break;
            }
            return ExecFFmpegCommand(strcmd, 1);
        }

        public static unsafe string av_strerror(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int error)
        {
            if (error < 0) throw new ApplicationException(av_strerror(error));
            return error;
        }
    }
}
