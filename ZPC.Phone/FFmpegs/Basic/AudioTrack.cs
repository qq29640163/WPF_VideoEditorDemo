using Newtonsoft.Json.Linq;
using System;

namespace ZPC.Phone
{
    public class AudioTrack
    {
        public string Codec { get; }
        public Bitrate Bitrate { get; set; }
        public byte Channels { get; }
        public string ChannelLayout { get; }
        public string Title { get; }
        public string Language { get; }
        public byte StreamIndex { get; }
        public long Size { get; set; }
        public bool Enabled { get; set; }
        public bool Default { get; set; }


        public AudioTrack(string codec, int bitrate, byte channels, string channelLayout, long size, byte streamIndex, string title, string language, bool defaultTrack = false)
        {
            Codec = codec;
            Bitrate = bitrate;
            Channels = channels;
            ChannelLayout = channelLayout;
            StreamIndex = streamIndex;
            Title = title;
            Language = language;
            Enabled = true;
            Default = defaultTrack;
            Size = size;
        }

        public static AudioTrack FromJson(JToken jsonElement)
        {
            double duration = 0;
            int bitrate = 0;
            byte channels = 0;
            string title = "", codec = "", language = "", channelLayout = "";
            bool defaultTrack = false;

            string codec_name = jsonElement.Value<string>("codec_name");
            if (string.IsNullOrEmpty(codec_name))
                codec = codec_name;

            string bit_rate = jsonElement.Value<string>("bit_rate");
            if (string.IsNullOrEmpty(bit_rate))
                bitrate = Convert.ToInt32(bit_rate);

            string _duration = jsonElement.Value<string>("duration");
            if (string.IsNullOrEmpty(_duration))
                duration = Convert.ToDouble(_duration);

            string _channels = jsonElement.Value<string>("channels");
            if (string.IsNullOrEmpty(_channels))
                channels = Convert.ToByte(_channels);

            string channel_layout = jsonElement.Value<string>("channel_layout");
            if (string.IsNullOrEmpty(channel_layout))
                channelLayout = channel_layout;

            byte index = jsonElement.Value<byte>("index");


            JToken e2 = jsonElement.Value<JToken>("tags");

            if (e2!=null &&e2.HasValues)
            {
                string _title = e2.Value<string>("title");
                string handler_name = e2.Value<string>("handler_name");

                if (string.IsNullOrEmpty(_title))
                    title = _title;
                else if (string.IsNullOrEmpty(handler_name) && handler_name != "SoundHandler") //FFProbe将忽略mp4文件上的title标记，因此为了显示此程序更改的标题，新标题也会放在处理程序标记中，FFProbe始终会报告该标记
                    title = handler_name;

                string _language = e2.Value<string>("language");
                if (_language != "und")
                    language = _language;
                if (bitrate == 0)
                {
                    string BPS_eng = e2.Value<string>("BPS-eng");
                    if (string.IsNullOrEmpty(BPS_eng))
                        bitrate = Convert.ToInt32(BPS_eng);
                }
                e2 = jsonElement.Value<JToken>("disposition");
                if (e2 != null && e2.HasValues)
                {
                    string _default = e2.Value<string>("default");
                    if (string.IsNullOrEmpty(_default))
                        defaultTrack = Convert.ToBoolean(Convert.ToByte(_default));
                }
            }

            long size = bitrate / 8 * Convert.ToInt32(duration);

            return new AudioTrack(codec, bitrate, channels, channelLayout, size, index, title, language, defaultTrack);
        }
    }
}