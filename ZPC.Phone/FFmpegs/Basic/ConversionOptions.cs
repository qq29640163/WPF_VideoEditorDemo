using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ZPC.Phone;

namespace ZPC.Phone
{
    [StructLayout(LayoutKind.Auto)]
    public struct ConversionOptions
    {
        public VideoEncoder Encoder;
        public bool NoAudio;
        public EncodingMode EncodingMode;
        public TimeIntervalCollection EncodeSections;
        /// <summary>
        /// 这个Key是AudioConversionOptions所指的audioTrack的streamIndex属性
        /// </summary>
        public Dictionary<int, AudioConversionOptions> AudioConversionOptions;
        public List<IFilter> Filters;
        public bool FadeEffect;

        public ConversionOptions(VideoEncoder encoder)
        {
            Encoder = encoder;
            NoAudio = false;
            EncodingMode = 0;
            EncodeSections = new TimeIntervalCollection(TimeSpan.Zero);
            AudioConversionOptions = new Dictionary<int, AudioConversionOptions>();
            Filters = new List<IFilter>();
            FadeEffect = false;
        }
    }

    public class AudioConversionOptions
    {
        public AudioEncoder Encoder;
        public string Title;
        public byte Channels;

        public AudioConversionOptions()
        {
        }
    }
}