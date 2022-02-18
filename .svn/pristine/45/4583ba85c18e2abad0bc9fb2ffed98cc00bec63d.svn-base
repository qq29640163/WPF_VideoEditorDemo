using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZPC.Phone.FFmpegs;

namespace ZPC.Phone
{
    public class MediaInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        public TimeSpan Duration { get; private set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; private set; }
        public string Codec { get; private set; }
        public string ExternalAudioCodec { get; private set; }
        /// <summary>
        /// 帧率
        /// </summary>
        public double Framerate { get; private set; }
        /// <summary>
        /// 比特率
        /// </summary>
        public Bitrate Bitrate { get; private set; }
        /// <summary>
        /// 源地址
        /// </summary>
        public string Source { get; private set; }
        public string ExternalAudioSource { get; private set; }
        /// <summary>
        /// 分辨率
        /// </summary>
        public Resolution Resolution { get; private set; }
        public ColorInfo ColorInfo { get; private set; }
        public IReadOnlyList<AudioTrack> AudioTracks => audioTracks;
        public int Width => Resolution.Width;
        public int Height => Resolution.Height;
        public bool IsLocal => !Source.StartsWith("http");
        public bool HasExternalAudio => !String.IsNullOrEmpty(ExternalAudioSource);
        public bool IsHDR => ColorInfo.DisplayMetadata != null;
        public string DynamicRange => IsHDR ? "HDR" : "SDR";
        public double BitsPerPixel => Bitrate.Bps / Framerate / (Resolution.Width * Resolution.Height);

        private AudioTrack[] audioTracks;


        private MediaInfo()
        {
        }

        public static async Task<MediaInfo> Open(string source)
        {
            MediaInfo mediaInfo = new MediaInfo();
            mediaInfo.Source = source;
            await mediaInfo.FF_Open(source).ConfigureAwait(false);
            if (!source.StartsWith("http")) mediaInfo.Title = Path.GetFileName(source);

            return mediaInfo;
        }

        public static async Task<MediaInfo> Open(string source, string externalAudioSource)
        {
            MediaInfo mediaInfo = new MediaInfo();
            mediaInfo.Source = source;
            await mediaInfo.FF_Open(source).ConfigureAwait(false);
            if (!source.StartsWith("http")) mediaInfo.Title = Path.GetFileName(source);
            mediaInfo.ExternalAudioSource = externalAudioSource;
            await mediaInfo.FF_ExternalAudioOpen(externalAudioSource).ConfigureAwait(false);

            return mediaInfo;
        }

        private async Task FF_Open(string source)
        {
            try
            {
                string jsonOutput = await FFprobeHelper.GetMediaInfo(source);
                await Task.Run(() => ParseJson(jsonOutput)).ConfigureAwait(false);
                ColorInfo = await GetColorInfo(source);
            }
            catch (Exception)
            {
                //由于无法从该线程创建窗口，因此每个异常都会被重试；调用此方法的人必须向用户显示错误
                throw;
            }       
        }

        private async Task FF_ExternalAudioOpen(string audioSource)
        {
            try
            {
                string jsonOutput = await FFprobeHelper.GetMediaInfo(audioSource);
                JToken jsonDocument = JsonConvert.DeserializeObject<JToken>(jsonOutput);
                JToken streamsElement = jsonDocument.Value<JToken>("streams");
                JToken audioStreamElement = null;

                // Find first audio stream
                foreach (var item in streamsElement.Children())
                {
                    string value = item.Value<string>("codec_type");
                    if (value  == "audio")
                    {
                        audioStreamElement = value;
                        break;
                    }
                }
                AudioTrack audioTrack = AudioTrack.FromJson(audioStreamElement);
                audioTracks = new AudioTrack[] { audioTrack };
                ExternalAudioCodec = audioTrack.Codec;
                Size += audioTrack.Size;
            }
            catch (Exception)
            {
                //由于无法从该线程创建窗口，因此每个异常都会被重试；调用此方法的人必须向用户显示错误
                throw;
            }
        }

        private void ParseJson(string jsonContent)
        {
            JToken jsonOutput = JsonConvert.DeserializeObject<JToken>(jsonContent);
            JToken streamsElement = jsonOutput.Value<JToken>("streams");
            JToken videoStreamElement = null;
            List<JToken> audioStreamElements = new List<JToken>();

            //查找第一个视频流和所有音频流
            foreach (var stream in streamsElement.Children())
            {
                if (stream.Value<string>("codec_type") == "video")
                {
                    if (videoStreamElement == null)
                        videoStreamElement = stream;
                }
                else if (stream.Value<string>("codec_type") == "audio")
                {
                    audioStreamElements.Add(stream);
                }
            }

            //获取分辨率
            short width = 0, height = 0;

            string codec_name = videoStreamElement.Value<string>("codec_name");
            if (string.IsNullOrEmpty(codec_name))
                Codec = codec_name;
            string _width = videoStreamElement.Value<string>("width");
            if (string.IsNullOrEmpty(_width))
                width = Convert.ToInt16(_width);
            string _height = videoStreamElement.Value<string>("height");
            if (string.IsNullOrEmpty(_height))
                height = Convert.ToInt16(_height);
            if (width > 0 && height > 0)
                Resolution = new Resolution(width, height);

            //获取帧率
            string fps = videoStreamElement.Value<string>("avg_frame_rate");
            if (fps != "N/A")
            {
                int a = Convert.ToInt32(fps.Remove(fps.IndexOf('/')));
                int b = Convert.ToInt32(fps.Remove(0, fps.IndexOf('/') + 1));
                Framerate = Math.Round(a / (double)b, 2);
            }

            // 获取其余属性
            JToken formatElement = jsonOutput.Value<JToken>("format");
            double totalSeconds = Double.Parse(formatElement.Value<string>("duration"));
            Duration = TimeSpan.FromSeconds(totalSeconds);
            Size = Int64.Parse(formatElement.Value<string>("size"));
            //由于有时视频流中缺少比特率，因此有必要从这里获得总比特率，并减去所有音频流的比特率
            Bitrate = Int32.Parse(formatElement.Value<string>("bit_rate"));

            //获取音频属性
            audioTracks = new AudioTrack[audioStreamElements.Count];
            for (int i = 0; i < audioStreamElements.Count; i++)
            {
                audioTracks[i] = AudioTrack.FromJson(audioStreamElements[i]);
                Bitrate -= audioTracks[i].Bitrate; //从总比特率中删除音频比特率以获得视频比特率
            }
        }

        private async Task<ColorInfo> GetColorInfo(string source)
        {
            string jsonOutput = await FFprobeHelper.GetColorInfo(source).ConfigureAwait(false);

            JToken JsonElement = JsonConvert.DeserializeObject<JToken>(jsonOutput);
            JToken element = JsonElement.Value<JToken>("frames").First;
            return ColorInfo.FromJson(element);
        }
    }
}