using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
namespace ZPC.Phone.FFmpegs
{
    public unsafe class AVFilterExaple
    {

        public string filter_descr = "scale=78:24,transpose=cclock";

        private static AVFormatContext* fmt_ctx;
        private static AVCodecContext* dec_ctx;
        private static AVFilterContext* buffersink_ctx;
        private static AVFilterContext* buffersrc_ctx;
        private AVFilterGraph* filter_graph;
        int video_stream_index = -1;
        long last_pts = ffmpeg.AV_NOPTS_VALUE;

        public int open_input_file(string filename)
        {
            AVCodec* dec;
            int ret;
            fmt_ctx = ffmpeg.avformat_alloc_context();
            var Fmt_Ctx = fmt_ctx;

            if ((ret = ffmpeg.avformat_open_input(&Fmt_Ctx, filename, null, null)) < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot open input file\n");
                return ret;
            }


            if ((ret = ffmpeg.avformat_find_stream_info(Fmt_Ctx, null)) < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot find stream information\n");
                return ret;
            }

            /* select the video stream */
            ret = ffmpeg.av_find_best_stream(Fmt_Ctx, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &dec, 0);
            if (ret < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot find a video stream in the input file\n");
                return ret;
            }
            video_stream_index = ret;
            /* create decoding context */
            dec_ctx = ffmpeg.avcodec_alloc_context3(dec);
            if (dec_ctx == null)
                return ffmpeg.AVERROR(ffmpeg.ENOMEM);
            ffmpeg.avcodec_parameters_to_context(dec_ctx, Fmt_Ctx->streams[video_stream_index]->codecpar);
            /* init the video decoder */
            if ((ret = ffmpeg.avcodec_open2(dec_ctx, dec, null)) < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot open video decoder\n");
                return ret;
            }
            return 0;
        }

        public int init_filters(string filters_descr)
        {
            string args = "";
            int ret = 0;
            
            AVFilter* buffersrc = ffmpeg.avfilter_get_by_name("buffer");
            AVFilter* buffersink = ffmpeg.avfilter_get_by_name("buffersink");
            AVFilterInOut* outputs = ffmpeg.avfilter_inout_alloc();
            AVFilterInOut* inputs = ffmpeg.avfilter_inout_alloc();
            AVRational time_base = fmt_ctx->streams[video_stream_index]->time_base;
            AVPixelFormat[] pix_fmts = { AVPixelFormat.AV_PIX_FMT_GRAY8, AVPixelFormat.AV_PIX_FMT_NONE };


            AVFilterGraph* Filter_Graph =ffmpeg.avfilter_graph_alloc();
            filter_graph = ffmpeg.avfilter_graph_alloc();

            if (false || false || true)
            {
                
            }

            if (((IntPtr)outputs) == IntPtr.Zero || ((IntPtr)inputs) == IntPtr.Zero || ((IntPtr)filter_graph) == IntPtr.Zero)
            {
                ret = ffmpeg.AVERROR(ffmpeg.ENOMEM);
                goto end;
            }
            /* buffer video source:the decoded frames from the decoder will be inserted here. */
            args = string.Format(
                "video_size="+dec_ctx->width*dec_ctx->height+":pix_fmt={0}:time_base={1}/{2}:pixel_aspect={3}/{4}"
                , new object[]
                {
                    dec_ctx->pix_fmt,time_base.num,time_base.den
                    ,dec_ctx->sample_aspect_ratio.num,dec_ctx->sample_aspect_ratio.den
                });

            var Buffersrc_Ctx = buffersrc_ctx;
            ret = ffmpeg.avfilter_graph_create_filter(&Buffersrc_Ctx, buffersrc, "in", args, null, filter_graph);
            if (ret < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot create buffer source\n");
                goto end;
            }

            /* buffer void sink : to teriminate the filter chain. */
            AVFilterContext* Buffersink_Ctx = buffersink_ctx;
            ret = ffmpeg.avfilter_graph_create_filter(&Buffersink_Ctx, buffersink, "out",
                                       null, null, filter_graph);
            if (ret < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot create buffer sink\n");
                goto end;
            }

            ret = ffmpeg.av_opt_set_int(buffersink_ctx, "pix_fmts",-1, ffmpeg.AV_OPT_SEARCH_CHILDREN);
            if (ret < 0)
            {
                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot set output pixel format\n");
                goto end;
            }

            /*
             * Set the endpoints for the filter graph. The filter_graph will
             * be linked to the graph described by filters_descr.
             */

            /*
             * The buffer source output must be connected to the input pad of
             * the first filter described by filters_descr; since the first
             * filter input label is not specified, it is set to "in" by
             * default.
             */
            outputs->name = ffmpeg.av_strdup("in");
            outputs->filter_ctx = Buffersrc_Ctx;
            outputs->pad_idx = 0;
            outputs->next = null;

            /*
             * The buffer sink input must be connected to the output pad of
             * the last filter described by filters_descr; since the last
             * filter output label is not specified, it is set to "out" by
             * default.
             */
            inputs->name = ffmpeg.av_strdup("out");
            inputs->filter_ctx = buffersink_ctx;
            inputs->pad_idx = 0;
            inputs->next = null;

            if ((ret = ffmpeg.avfilter_graph_parse_ptr(filter_graph, filters_descr,
                                            &inputs, &outputs, null)) < 0)
                goto end;

            if ((ret = ffmpeg.avfilter_graph_config(filter_graph, null)) < 0)
                goto end;
            end:
            ffmpeg.avfilter_inout_free(&inputs);
            ffmpeg.avfilter_inout_free(&inputs);
            return ret;
        }

        public void display_frame(AVFrame* frame, AVRational time_base)
        {
            int x, y;
            uint* p0,p;
            long delay;

            if (frame->pts != ffmpeg.AV_NOPTS_VALUE)
            {
                if (last_pts != ffmpeg.AV_NOPTS_VALUE)
                {
                    AVRational AV_TIME_BASE_Q;
                    AV_TIME_BASE_Q = new AVRational { den = 1, num = ffmpeg.AV_TIME_BASE };
                    /* sleep roughly the right amount of time;
                     * usleep is in microseconds, just like AV_TIME_BASE. */
                    delay = ffmpeg.av_rescale_q(frame->pts - last_pts,
                                         time_base, AV_TIME_BASE_Q);
                    if (delay > 0 && delay < 1000000)
                        Thread.Sleep((int)(delay/1000));
                }
                last_pts = frame->pts;
            }

            /* Trivial ASCII grayscale display. */
            p0 = (uint*)frame->data[0];
            //puts("\033c");
            //for (y = 0; y < frame->height; y++)
            //{
            //    p = p0;
            //    for (x = 0; x < frame->width; x++)
            //        putchar(" .-+#"[*(p++) / 52]);
            //    putchar('\n');
            //    p0 += frame->linesize[0];
            //}
            //fflush(stdout);
        }

        public int run()
        {
            int ret;
            AVPacket* packet;
            AVFrame* frame;
            AVFrame* filt_frame;

            //if (argc != 2)
            //{
            //    fprintf(stderr, "Usage: %s file\n", argv[0]);
            //    exit(1);
            //}

            frame = ffmpeg.av_frame_alloc();
            filt_frame = ffmpeg.av_frame_alloc();
            packet = ffmpeg.av_packet_alloc();
            if (frame==null || filt_frame==null || packet==null)
            {
                Trace.TraceError("Could not allocate frame or packet");
                return 0;
            }

            if ((ret = open_input_file("E:\\LOL.mp4")) < 0)
                goto end;
            if ((ret = init_filters(filter_descr)) < 0)
                goto end;

            /* read all packets */
            while (true)
            {
                if ((ret = ffmpeg.av_read_frame(fmt_ctx, packet)) < 0)
                    break;

                if (packet->stream_index == video_stream_index)
                {
                    ret = ffmpeg.avcodec_send_packet(dec_ctx, packet);
                    if (ret < 0)
                    {
                        ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Error while sending a packet to the decoder\n");
                        break;
                    }

                    while (ret >= 0)
                    {
                        ret = ffmpeg.avcodec_receive_frame(dec_ctx, frame);
                        if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                        {
                            break;
                        }
                        else if (ret < 0)
                        {
                            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Error while receiving a frame from the decoder\n");
                            goto end;
                        }

                        frame->pts = frame->best_effort_timestamp;

                        /* push the decoded frame into the filtergraph */
                        if (ffmpeg.av_buffersrc_add_frame_flags(buffersrc_ctx, frame,8) < 0)
                        {
                            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Error while feeding the filtergraph\n");
                            break;
                        }

                        /* pull filtered frames from the filtergraph */
                        while (true)
                        {
                            ret = ffmpeg.av_buffersink_get_frame(buffersink_ctx, filt_frame);
                            if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                                break;
                            if (ret < 0)
                                goto end;
                            display_frame(filt_frame, buffersink_ctx->inputs[0]->time_base);
                            ffmpeg.av_frame_unref(filt_frame);
                        }
                        ffmpeg.av_frame_unref(frame);
                    }
                }
                ffmpeg.av_packet_unref(packet);
            }
        end:
            AVFilterGraph* Filter_Graph= filter_graph;
            AVCodecContext* Dec_Ctx= dec_ctx;
            AVFormatContext* Fmt_Ctx= fmt_ctx;
            ffmpeg.avfilter_graph_free(&Filter_Graph);
            ffmpeg.avcodec_free_context(&Dec_Ctx);
            ffmpeg.avformat_close_input(&Fmt_Ctx);
            ffmpeg.av_frame_free(&frame);
            ffmpeg.av_frame_free(&filt_frame);
            ffmpeg.av_packet_free(&packet);

            if (ret < 0 && ret != ffmpeg.AVERROR_EOF)
            {
                byte s = Convert.ToByte(0);

                byte* b = &s;
                var result=ffmpeg.av_make_error_string(b, ffmpeg.AV_ERROR_MAX_STRING_SIZE, ret);
                byte r = *result;


                Trace.TraceError("Error occurred: %s\n", r.ToString());
                return 1;
            }

            return 0;
        }
     }
}
