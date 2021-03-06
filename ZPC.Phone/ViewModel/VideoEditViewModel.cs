using CommonServiceLocator;
using FFmpeg.AutoGen;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using ZPC.Phone.Enum;
using ZPC.Phone.FFmpegs;
using ZPC.Phone.FFmpegs.Filters;
using ZPC.Phone.Model;
using ZPC.Phone.View;

namespace ZPC.Phone.ViewModel
{
    public class VideoEditViewModel : BindableBase, INavigationAware
    {

        #region Command
        public DelegateCommand OpenSrcCommand { get; set; }

        public DelegateCommand OpenTagCommand { get; set; }

        public DelegateCommand PlayCommand { get; set; }

        public DelegateCommand StopCommand { get; set; }

        public DelegateCommand<object> LoadCommand { get; set; }

        public DelegateCommand<object> CutStartCommand { get; set; }

        public DelegateCommand<object> CutEndCommand { get; set; }

        public DelegateCommand ExecCutCommand { get; set; }

        public DelegateCommand<object> SelectChangeCommand { get; set; }

        public DelegateCommand ExecMergeCommand { get; set; }

        public DelegateCommand OpenPopCommand { get; set; }

        public DelegateCommand AddWaterMaskCommand { get; set; }

        public DelegateCommand AddTextCommand { get; set; }

        public DelegateCommand ClearSrcCommand { get; set; }

        public DelegateCommand ConvertGifCommand { get; set; }

        public DelegateCommand AddBlurCommand { get; set; }

        public DelegateCommand<object> AddEffectsCommand { get; set; }

        public DelegateCommand ClearWaterMarkCommand { get; set; }

        public DelegateCommand ExecAddWaterMarkCommand { get; set; }

        #endregion

        #region NotifyProperty

        private Style _buttonMedieStyle = System.Windows.Application.Current?.FindResource("ButtonMediaPlay") as Style;
        public Style ButtonMedieStyle
        {
            get { return _buttonMedieStyle; }
            set { SetProperty(ref _buttonMedieStyle, value); }
        }

        private List<ComboBoxItem> _stretchModeItems;
        public List<ComboBoxItem> StretchModeItems
        {
            get { return _stretchModeItems; }
            set { SetProperty(ref _stretchModeItems, value); }
        }

        private ComboBoxItem _selectItem;

        public ComboBoxItem SelectItem
        {
            get { return _selectItem; }
            set { SetProperty(ref _selectItem, value); }
        }
        private bool _isPlay = true;

        public bool IsPlay
        {
            get { return _isPlay; }
            set { SetProperty(ref _isPlay, value); }
        }

        private string _txtSrcAdress;
        public string TxtSrcAdress
        {
            get { return _txtSrcAdress; }
            set { SetProperty(ref _txtSrcAdress, value); }
        }

        private string _txtTagAdress;
        public string TxtTagAdress
        {
            get { return _txtTagAdress; }
            set { SetProperty(ref _txtTagAdress, value); }
        }

        private double _volumeValue=0.5;

        public double VolumeValue
        {
            get { return _volumeValue; }
            set { SetProperty(ref _volumeValue, value, VolumeValueChange); }
        }

        private string _volumeString = "音量:0%";

        private double _progress = 0;
        public double Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        public string VolumeString
        {
            get { return _volumeString; }
            set { SetProperty(ref _volumeString,value); }
        }

        /// <summary>
        /// 音量值变化
        /// </summary>
        private void VolumeValueChange()
        {
            if (media == null) return;
            media.Volume = VolumeValue;
            VolumeString = "音量:" + (VolumeValue*100).ToString("f0") + "%";
        }
        private double _speedRatioValue = 1;

        public double SpeedRatioValue
        {
            get { return _speedRatioValue; }
            set { SetProperty(ref _speedRatioValue, value,SpeedRatioValueChange); }
        }

        /// <summary>
        /// 播放速度变化
        /// </summary>
        private void SpeedRatioValueChange()
        {
            media.SpeedRatio = SpeedRatioValue;
        }

        private string _time;

        public string Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        private string textCutStartTime;
        public string TextCutStartTime
        {
            get { return textCutStartTime; }
            set { SetProperty(ref textCutStartTime, value); }
        }

        private string textCutEndTime;
        public string TextCutEndTime
        {
            get { return textCutEndTime; }
            set { SetProperty(ref textCutEndTime, value); }
        }


        private bool _execCutIsEnable = true;
        public bool ExecCutIsEnable
        {
            get { return _execCutIsEnable; }
            set { SetProperty(ref _execCutIsEnable, value); }
        }

        private bool _execMergeIsEnable = true;
        public bool ExecMergeIsEnable
        {
            get { return _execMergeIsEnable; }
            set { SetProperty(ref _execMergeIsEnable, value); }
        }

        private bool _execLogPopIsOpen = false;

        public bool ExecLogPopIsOpen
        {
            get { return _execLogPopIsOpen; }
            set { SetProperty(ref _execLogPopIsOpen, value); }
        }

        private string textExecLog;

        public string TextExecLog
        {
            get { return textExecLog; }
            set { SetProperty(ref textExecLog, value); }
        }

        //private List<string> _targetScale = new List<string>();

        //public List<string> TargetScale
        //{
        //    get { return _targetScale; }
        //    set { SetProperty(ref _targetScale, value); }
        //}

        public ObservableCollection<string> TargetScale { get; set; } = new ObservableCollection<string>();

        private string _selectScale;

        public string SelectScale
        {
            get { return _selectScale; }
            set { SetProperty(ref _selectScale, value); }
        }

        //private List<string> _listVideoSrc = new List<string>();

        //public List<string> ListVideoSrc
        //{
        //    get { return _listVideoSrc; }
        //    set 
        //    { 
        //        SetProperty(ref _listVideoSrc, value); 
        //    }
        //}
        public ObservableCollection<string> ListVideoSrc { get; set; } = new ObservableCollection<string>() { };

        private string _selectVideo;

        public string SelectVideo
        {
            get { return _selectVideo; }
            set 
            { 
                SetProperty(ref _selectVideo, value); 
            }
        }

        private int _textCount=0;
        public int TextCount
        {
            get { return _textCount; }
            set 
            {
                SetProperty(ref _textCount, value);
            }
        }

        private int _imageCount = 0;
        public int ImageCount
        {
            get { return _imageCount; }
            set
            {
                SetProperty(ref _imageCount, value);
            }
        }


        #endregion

        private MediaElement media;
        private DispatcherTimer timer = null;
        private VideoEditView videoEditView;
        private TimeSpan Startts;
        private TimeSpan Endts;
        private IRegionManager _regionManager;
        private IDialogService _dialogService;
        private List<IFilter> filters = new List<IFilter>();
        private List<Watermark> watermarks = new List<Watermark>();
        private MediaInfo mediaInfo;
        private string cmd;
        private TimeSpan TotalTime;
        /// <summary>
        /// 构造函数
        /// </summary>
        public VideoEditViewModel()
        {
            StretchModeItems = new List<ComboBoxItem>()
            {
                new ComboBoxItem(){Content=Stretch.None },
                new ComboBoxItem(){Content=Stretch.Fill },
                new ComboBoxItem(){Content=Stretch.Uniform,IsSelected=true },
                new ComboBoxItem(){Content=Stretch.UniformToFill },
            };
            SelectItem = StretchModeItems[2];
            OpenSrcCommand = new DelegateCommand(OnOpenSrc);
            OpenTagCommand = new DelegateCommand(OnOpenTag);
            PlayCommand = new DelegateCommand(OnPlay);
            StopCommand = new DelegateCommand(OnStop);
            LoadCommand = new DelegateCommand<object>(OnLoad);
            CutStartCommand = new DelegateCommand<object>(OnCutStart);
            CutEndCommand = new DelegateCommand<object>(OnCutEnd);
            ExecCutCommand = new DelegateCommand(OnExecCut);
            SelectChangeCommand = new DelegateCommand<object>(OnSelectChange);
            ExecMergeCommand = new DelegateCommand(OnExecMerge);
            OpenPopCommand = new DelegateCommand(OnOpenPop);
            AddWaterMaskCommand = new DelegateCommand(OnAddWaterMask);
            AddTextCommand = new DelegateCommand(OnAddText);
            ClearSrcCommand = new DelegateCommand(OnClearSrc);
            ConvertGifCommand = new DelegateCommand(OnConvertGifCommand);
            AddBlurCommand = new DelegateCommand(OnAddBlur);
            AddEffectsCommand = new DelegateCommand<object>(OnAddEffects); 
            ClearWaterMarkCommand = new DelegateCommand(OnClearWater);
            ExecAddWaterMarkCommand = new DelegateCommand(OnExecAddWater);
            FFmpegHelper.OutPutFFmpegCommadExecEvent += FFmpegHelper_OutPutFFmpegCommadExecEvent;
            FFmpegHelper.ProgressChanged += FFmpegHelper_ProgressChanged;
            //string SourcePath=@"D:\LOL.mp4";
            //string TargetPath = @"D:\LOL2.mp4";

            //string result = FFmpegHelper.ExecFFmpegCommand(FFmpegEnum.Cut_FromToSecond, new object[] { "0:10", "5" }, SourcePath, TargetPath);
        }

        public VideoEditViewModel(IRegionManager regionManager,IDialogService dialogService)
            :this()
        {
            this._regionManager = regionManager;
            this._dialogService = dialogService;
        }



        #region Event方法
        private void TimelineSlider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (media == null) return;
            media.Pause();
        }

        private void TimelineSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (media == null) return;
            media.Play();
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double position = e.NewValue;
            if (media == null) return;
            media.Position = TimeSpan.FromSeconds(position);
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            videoEditView.timelineSlider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            media.Volume = VolumeValue;
            VolumeString = "音量:" + (VolumeValue * 100).ToString("f0") + "%";
            //媒体文件打开成功
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Time = media.Position.ToString(@"hh\:mm\:ss");
            videoEditView.timelineSlider.Value = media.Position.TotalSeconds;
            VolumeValue = media.Volume;
            SpeedRatioValue = media.SpeedRatio;
        }

        private void FFmpegHelper_OutPutFFmpegCommadExecEvent(string log)
        {

            /*"fps：表示平均帧率，总帧数除以总时长（以s为单位）\r\n"
                + "tbr 表示帧率，该参数倾向于一个基准，往往tbr跟fps相同。\r\n"
                + "tbn 表示视频流 timebase（时间基准），比如ts流的timebase 为90000，flv格式视频流timebase为1000\r\n"
                + "tbc 表示视频流codec timebase ，对于264码流该参数通过解析sps间接获取（通过sps获取帧率）\r\n"*/
            TextExecLog += log;
        }

        private void FFmpegHelper_ProgressChanged(ProgressData obj)
        {
            if (double.IsNaN(obj.CurrentTime.TotalSeconds)) return;
            if (TotalTime.TotalSeconds == 0) return;
            Progress = ((double)decimal.Round(decimal.Parse((obj.CurrentTime.TotalSeconds / TotalTime.TotalSeconds * 100).ToString()), 2));
        }
        #endregion

        #region Commad 方法
        private void OnLoad(object a)
        {
            RoutedEventArgs args = a as RoutedEventArgs;
            videoEditView = args.OriginalSource as VideoEditView;
            media = videoEditView.media;
            media.MediaOpened += Media_MediaOpened;
            videoEditView.timelineSlider.ValueChanged += TimelineSlider_ValueChanged;

            videoEditView.timelineSlider.PreviewMouseLeftButtonUp += TimelineSlider_PreviewMouseLeftButtonUp;
            videoEditView.timelineSlider.PreviewMouseLeftButtonDown += TimelineSlider_PreviewMouseLeftButtonDown;
        }
        
        private async void OpenSrc(string txt)
        {
            mediaInfo = await MediaInfo.Open(txt);
        }

        private void OnOpenSrc()
        {
            //Run();
            //return;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择视频文件";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "MP4|*.mp4|AVI|*.avi|WMV|*.wmv|MPG|*.mpeg;*.mpg";
            //if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    TxtSrcAdress = string.IsNullOrEmpty(TxtSrcAdress) ? openFileDialog.FileName : string.Join("|", TxtSrcAdress, openFileDialog.FileName);
            //    OpenSrc(TxtSrcAdress);
            //}
            //return;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ListVideoSrc.Add(openFileDialog.FileName);
                SelectVideo = openFileDialog.FileName;
                TxtSrcAdress = string.IsNullOrEmpty(TxtSrcAdress) ? openFileDialog.FileName : string.Join("|", TxtSrcAdress, openFileDialog.FileName);
                Task<string> vs = new Task<string>(() => {
                    //获取打开视频分辨率
                    OpenSrc(openFileDialog.FileName);
                    string info = FFmpegHelper.ExecFFprobeCommand(FFprobeEnum.GetVideoResolution, null, openFileDialog.FileName, null);

                    string[] Scales = info.Split(new string[] { "&Exists Complete!" }, StringSplitOptions.RemoveEmptyEntries);
                    string Scale = string.Empty;
                    if (Scales.Length > 0)
                    {
                        Scale = Scales[0];
                        if (!TargetScale.Contains(Scale))
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() => 
                            {
                                TargetScale.Add(Scale);
                            });
                            
                        }
                        if (string.IsNullOrEmpty(SelectScale))
                            SelectScale = Scale;
                    }
                    return Scale;
                });
                vs.Start();
            }
        }

        private void OnOpenTag()
        {
            if (string.IsNullOrEmpty(TxtSrcAdress))
            {
                System.Windows.MessageBox.Show("请先导入源视频", "提示");
                return;
            }

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.Description = "选择目标路径";
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                    System.Windows.MessageBox.Show("文件夹路径不能为空", "提示");
                    return;
                }
                string outfile = "outVideo." + SelectVideo.Split(new char[] { '.' })[1];
                TxtTagAdress = folderDialog.SelectedPath.EndsWith("\\") ? (folderDialog.SelectedPath + outfile) : (folderDialog.SelectedPath + "\\" + outfile);
            }
        }

        private void OnPlay()
        {
            media.Volume = VolumeValue;
            media.SpeedRatio = SpeedRatioValue;
            if (IsPlay)
            {
                IsPlay = false;
                ButtonMedieStyle = System.Windows.Application.Current?.FindResource("ButtonMediaPause") as Style;
                media.Play();
            }
            else
            {
                IsPlay = true;
                ButtonMedieStyle = System.Windows.Application.Current?.FindResource("ButtonMediaPlay") as Style;
                media.Pause();
            }
        }

        private void OnStop()
        {
            IsPlay = true;
            ButtonMedieStyle = System.Windows.Application.Current?.FindResource("ButtonMediaPlay") as Style;
            media.Close();
            media.Stop();
        }

        private void OnCutStart(object value)
        {
            if (string.IsNullOrEmpty(TxtSrcAdress))
            {
                System.Windows.MessageBox.Show("请先导入源视频");
                return;
            }
            TextCutStartTime = value.ToString();
            Startts = TimeSpan.Parse(TextCutStartTime);
        }

        private void OnCutEnd(object value)
        {
            if(string.IsNullOrEmpty(TxtSrcAdress))
            {
                System.Windows.MessageBox.Show("请先导入源视频");
                return;
            }
            TextCutEndTime = value.ToString();

            if (Startts == null)
            {
                System.Windows.MessageBox.Show("请设置切割开始时间");
                return;
            }
            Endts = TimeSpan.Parse(TextCutEndTime);
        }

        private void OnExecCut()
        {
            var CanCut = Endts.CompareTo(Startts);

            if (string.IsNullOrEmpty(TxtSrcAdress))
            {
                System.Windows.MessageBox.Show("请设置源路径");
                return;
            }

            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置目标路径");
                return;
            }

            if (CanCut != 1)
            {
                return;
            }
            double cutSeconds= Endts.Subtract(Startts).TotalSeconds;
            TotalTime = Endts.Subtract(Startts);
            Task<string> tast = new Task<string>(() => 
            {
                ExecCutIsEnable = false;
                ExecLogPopIsOpen = true;
                var result = FFmpegHelper.ExecFFmpegCommand(FFmpegEnum.Cut_FromToSecond, new object[] { TextCutStartTime, cutSeconds }, SelectVideo, TxtTagAdress);
                if (File.Exists(TxtTagAdress))
                {
                    ExecCutIsEnable = true;
                    ExecLogPopIsOpen = false;
                    FileInfo info = new FileInfo(TxtTagAdress);
                    System.Diagnostics.Process.Start("explorer.exe", info.Directory.ToString());
                    Progress = 100;
                    System.Windows.MessageBox.Show("剪辑完成！");
                }
                return result;
            });
            tast.Start();
        }

        private void OnSelectChange(object value)
        {
            if (media == null) return;
            ComboBoxItem item = value as ComboBoxItem;
            media.SpeedRatio = Convert.ToDouble(item.Content);
        }

        private void OnExecMerge()
        {
            if (string.IsNullOrEmpty(TxtSrcAdress) || !TxtSrcAdress.Contains("|"))
            {
                System.Windows.MessageBox.Show("请导入2个及以上的源视频");
                return;
            }
            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出视频的路径");
                return;
            }
            if (string.IsNullOrEmpty(SelectScale))
            {
                System.Windows.MessageBox.Show("请设置合成视频的分辨率");
                return;
            }
            TotalTime = TimeSpan.Zero;
            Task<string> tast = new Task<string>(() =>
            {
                ExecMergeIsEnable = false;
                ExecLogPopIsOpen = true;

                //获取打开视频比特率
                string FFprobeinfo = string.Empty;
                List<int> bitrates = new List<int>();
                string[] strSrcAddress = TxtSrcAdress.Split('|');
                foreach (var src in strSrcAddress)
                {
                    OpenSrc(src);
                    TotalTime =TotalTime.Add(mediaInfo.Duration);
                    FFprobeinfo = FFmpegHelper.ExecFFprobeCommand(FFprobeEnum.GetVideoBitrate, null, src, null);
                    string[] str = FFprobeinfo.Split(new string[] { "&Exists Complete!" }, StringSplitOptions.RemoveEmptyEntries);
                    if (str.Length > 0)
                    {
                        FFprobeinfo = str[0];
                        var json = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(FFprobeinfo);
                        var Child = json.Value<Newtonsoft.Json.Linq.JToken>("format");
                        int bitrate = Convert.ToInt32(Child.Value<string>("bit_rate"));
                        bitrates.Add(bitrate);
                    }
                }
                var result = FFmpegHelper.ExecFFmpegCommand(FFmpegEnum.Merge_Video_FromDiffScale, new object[] { SelectScale, bitrates.Max() }, TxtSrcAdress, TxtTagAdress);
                if (File.Exists(TxtTagAdress))
                {
                    ExecMergeIsEnable = true;
                    ExecLogPopIsOpen = false;
                    FileInfo info = new FileInfo(TxtTagAdress);
                    System.Diagnostics.Process.Start("explorer.exe", info.Directory.ToString());
                    Progress = 100;
                    System.Windows.MessageBox.Show("合并完成！");
                }
                return result;
            });
            tast.Start();

        }

        private void OnOpenPop()
        {
            ExecLogPopIsOpen = !ExecLogPopIsOpen;
        }

        private void OnAddText() 
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入视频的路径");
                return;
            }
            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出视频的路径");
                return;
            }
            _dialogService.ShowDialog(DialogName.WatermarkTextSetting.ToString(), null, r =>
            {
                if (r.Result == ButtonResult.Yes)
                {
                    Task task = new Task(() =>
                    {
                        var watermark = r.Parameters.GetValue<Watermark>("Watermark");
                        TextCount++;
                        watermarks.Add(watermark);
                    });
                    task.Start();

                }
            });
        } 
        private void OnAddWaterMask() 
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入视频的路径");
                return;
            }
            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出视频的路径");
                return;
            }
            _dialogService.ShowDialog(DialogName.WatermarkSetting.ToString(), null, r=> 
            {
                if (r.Result == ButtonResult.Yes)
                {
                    var watermark = r.Parameters.GetValue<Watermark>("Watermark");
                    ImageCount++;
                    watermarks.Add(watermark);
                }
            });
        }

        private void OnAddBlur()
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入源视频！");
                return;
            }

            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出路径！");
                return;
            }
            _dialogService.ShowDialog(DialogName.Blur.ToString(), null, r =>
            {
                if (r.Result == ButtonResult.Yes)
                {
                    object[] GifParameters = r.Parameters.GetValue<object[]>("BlurParameter");
                    OpenSrc(SelectVideo);
                    TotalTime = mediaInfo.Duration;
                    Task task = new Task(() =>
                    {
                        ExecLogPopIsOpen = true;
                        var result = FFmpegHelper.ExecFFmpegCommand
                            (FFmpegEnum.BlurDesignatedArea
                            , new object[]
                            {
                                GifParameters[0].ToString(),
                                GifParameters[1].ToString(),
                                GifParameters[2].ToString(),
                                GifParameters[3].ToString(),
                                GifParameters[4].ToString(),
                                GifParameters[5].ToString(),
                            }
                            , SelectVideo
                            , TxtTagAdress);
                        if (File.Exists(TxtTagAdress))
                        {
                            ExecMergeIsEnable = true;
                            ExecLogPopIsOpen = false;
                            FileInfo info = new FileInfo(TxtTagAdress);
                            System.Diagnostics.Process.Start("explorer.exe", info.Directory.ToString());
                            System.Windows.MessageBox.Show("模糊添加完成！");
                        }
                    });
                    task.Start();

                }
            });
        }

        private void OnClearSrc()
        {
            ListVideoSrc.Clear();
            TargetScale.Clear();
            TxtSrcAdress = string.Empty;
        }

        private void OnConvertGifCommand()
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入源视频！");
                return;
            }

            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出路径！");
                return;
            }
            TxtTagAdress = TxtTagAdress.Replace(".mp4", ".gif");
            OpenSrc(SelectVideo);
            TotalTime = mediaInfo.Duration;
            _dialogService.ShowDialog(DialogName.GifSet.ToString(), null, r =>
            {
                if (r.Result == ButtonResult.Yes)
                {
                    object[] GifParameters = r.Parameters.GetValue<object[]>("GifParameter");

                    Task task = new Task(() =>
                    {
                        ExecLogPopIsOpen = true;
                        var result = FFmpegHelper.ExecFFmpegCommand
                            (FFmpegEnum.ConvertGif
                            , new object[]
                            {
                                GifParameters[0].ToString(),
                                GifParameters[1].ToString(),
                                GifParameters[2].ToString()+"x"+GifParameters[3].ToString(),
                                GifParameters[4].ToString(),
                            }
                            , SelectVideo
                            , TxtTagAdress);
                        if (File.Exists(TxtTagAdress))
                        {
                            ExecMergeIsEnable = true;
                            ExecLogPopIsOpen = false;
                            FileInfo info = new FileInfo(TxtTagAdress);
                            System.Diagnostics.Process.Start("explorer.exe", info.Directory.ToString());
                            System.Windows.MessageBox.Show("GIF转化完成！");
                        }
                    });
                    task.Start();

                }
            });
            //FFmpegHelper.ExecFFmpegCommand(FFmpegEnum.ConvertGif, new object[] { "00:00:00","00,00,05","200x150","15" }, SelectVideo, TxtTagAdress);
        }

        private void OnAddEffects(object type)
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入视频的路径");
                return;
            }

            int t = Convert.ToInt32(type);
            string filter = "";
            switch (t)
            {
                case 0:
                    filter = "fade=in:0:90";
                    break;
                case 1:
                    filter = "lutyuv='u=128:v=128'";
                    break;
                case 2:
                    filter = "unsharp=luma_msize_x=7:luma_msize_y=7:luma_amount=2.5";
                    break;
                case 3:
                    filter = "edgedetect=low=0.1:high=0.1";
                    break;
                case 4:
                    filter = "lutyuv='y = 2 * val'";
                    break;
                case 5:
                    filter = "colorbalance=rh=-0.6";
                    break;
                case 6:
                    filter = "lutyuv='y = negval:u = negval:v = negval'";
                    break;
            }
            filter = "-vf \"" + filter + "\"";
            FFmpegHelper.ExecFFplayCommand(SelectVideo, filter);
        }

        private void OnClearWater()
        {
            ImageCount = 0;
            TextCount = 0;
            watermarks.Clear();
            filters.Clear();
        }

        private void OnExecAddWater()
        {
            if (string.IsNullOrEmpty(SelectVideo))
            {
                System.Windows.MessageBox.Show("请设置导入视频的路径");
                return;
            }
            if (string.IsNullOrEmpty(TxtTagAdress))
            {
                System.Windows.MessageBox.Show("请设置导出视频的路径");
                return;
            }
            if (watermarks.Count == 0)
            {
                System.Windows.MessageBox.Show("请添加至少一个水印");
                return;
            }
            OpenSrc(SelectVideo);
            TotalTime = mediaInfo.Duration;
            string filter = "";
            string inputFlag = "";
            string outputFlag = "";
            DrawtextFilter drawtextFilter = null;
            int i = 0;
            foreach (var watermark in watermarks.Where(w=> { return w.IsImage == false; }))
            {
                var LastWatermark = watermarks.Where(w => { return w.IsImage == false; }).Last();
                if (drawtextFilter == null)
                {
                    drawtextFilter = new DrawtextFilter();
                    drawtextFilter.InputFlag = "0:v";
                }
                else
                {
                    outputFlag = drawtextFilter.OutputFlag;
                    drawtextFilter = new DrawtextFilter();
                    drawtextFilter.InputFlag = outputFlag;
                }
                i++;
                drawtextFilter.Fontfile = watermark.FontFamily;
                drawtextFilter.Fontsize = watermark.FontSize.ToString();
                drawtextFilter.Fontcolor = watermark.FontColor;
                drawtextFilter.Text = watermark.Text;
                drawtextFilter.X = watermark.WatermarkLocation.X.ToString();
                drawtextFilter.Y = watermark.WatermarkLocation.Y.ToString();
                drawtextFilter.OutputFlag = "TextOut" + i.ToString();
                if (watermark.EndTime != TimeSpan.Zero)
                {
                    drawtextFilter.StartTime = watermark.StartTime.TotalSeconds.ToString();
                    drawtextFilter.EndTime = watermark.EndTime.TotalSeconds.ToString();
                }
                if (watermarks.Where(w => { return w.IsImage == false; }).Count() ==1
                && watermarks.Where(w => { return w.IsImage == true; }).Count() == 0)
                {
                    if (LastWatermark == watermark)
                    {
                        drawtextFilter.OutputFlag = "";
                    }
                    filter += drawtextFilter.GetFilter();
                }
                else
                    filter += drawtextFilter.GetFilter() + ";";
            }

            if (watermarks.Where(w => { return w.IsImage == false; }).Count() > 1
                && watermarks.Where(w => { return w.IsImage == true; }).Count() == 0)
            {
                filter = filter.Remove(filter.Length - 1, 1);
            }


            inputFlag = "TextOut" + i.ToString();
            i = 0;
            MovieFilter movieFilter = null;
            OverlayFilter overlayFilter = null;
            foreach (var watermark in watermarks.Where(w => { return w.IsImage == true; }))
            {
                var LastWatermark = watermarks.Where(w => { return w.IsImage == true; }).Last();

                i++;
                movieFilter = new MovieFilter(watermark.ImagePath);
                movieFilter.OutputFlag = $"wm{i}";
                overlayFilter = new OverlayFilter();
                if (!string.IsNullOrEmpty(filter))
                {
                    overlayFilter.InputFlag = inputFlag;
                }
                
                overlayFilter.OverlayFlag = movieFilter.OutputFlag;
                overlayFilter.OutputFlag = $"ImageOut{i}";
                overlayFilter.X = watermark.WatermarkLocation.X.ToString();
                overlayFilter.Y = watermark.WatermarkLocation.Y.ToString();
                inputFlag = overlayFilter.OutputFlag;
                Resolution res = new Resolution(Convert.ToInt16(watermark.ImageSize.Width), Convert.ToInt16(watermark.ImageSize.Height));
                ScaleFilter scaleFilter = new ScaleFilter(res);
                if (scaleFilter.OutputResolution.Width > 0 || scaleFilter.OutputResolution.Width > 0)
                {
                    scaleFilter.OutputFlag = movieFilter.OutputFlag;
                    movieFilter.OutputFlag = "";
                }
                if (watermark.EndTime != TimeSpan.Zero)
                {
                    overlayFilter.StartTime = watermark.StartTime.TotalSeconds.ToString();
                    overlayFilter.EndTime = watermark.EndTime.TotalSeconds.ToString();
                }
                if (watermark == LastWatermark)
                {
                    if (watermarks.Where(w => { return w.IsImage == false; }).Count() == 0
                        && watermarks.Where(w => { return w.IsImage == true; }).Count() == 1)
                    {
                        inputFlag = "0:v";
                    }
                    overlayFilter.OutputFlag = "";
                }

                filter += movieFilter.GetFilter() + (string.IsNullOrEmpty(scaleFilter.GetFilter()) ? "" : "," + scaleFilter.GetFilter()) + ";" + overlayFilter.GetFilter() + (LastWatermark == watermark ? "" : ";");
            }
            Task task = new Task(() =>
             {
                 ExecLogPopIsOpen = true;
                 var result = FFmpegHelper.ExecFilter(true, SelectVideo, TxtTagAdress, filter);
                 if (File.Exists(TxtTagAdress))
                 {
                     ExecMergeIsEnable = true;
                     FileInfo info = new FileInfo(TxtTagAdress);
                     System.Diagnostics.Process.Start("explorer.exe", info.Directory.ToString());
                     Progress = 100;
                     System.Windows.MessageBox.Show("水印添加完成！");
                 }
             });
            task.Start();
        }

        #endregion

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        #region FFmpeg AutoGen相关方法
        
        private void Run()
        {
            
            //输出当前目录和windows位数，然后注册ffmpeg给AutoGen
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            //输出ffmpeg的版本
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");

            //配置ffmpeg输出日志
            SetupLogging();
            //FFmpegs.AVFilterExaple a = new FFmpegs.AVFilterExaple();
            //a.run();
            //配置硬件解码器
            ConfigureHWDecoder(out var deviceType);

            //解码
            Console.WriteLine("Decoding...");
            DecodeAllFramesToImages(deviceType);

            //编码
            Console.WriteLine("Encoding...");
            EncodeImagesToH264();
        }

        /// <summary>
        /// 配置日志
        /// </summary>
        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            //不转化为本地函数
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
              {
                  if (level > ffmpeg.av_log_get_level()) return;
                  var lineSize = 1024;
                  var lineBuffer = stackalloc byte[lineSize];
                  var printPrefix = 1;
                  ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                  var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                  Console.ForegroundColor = ConsoleColor.Yellow;
                  Console.Write(line);
                  Console.ResetColor();
              };
            ffmpeg.av_log_set_callback(logCallback);
        }

        /// <summary>
        /// 配置硬件解码器 hardware硬件的意思
        /// </summary>
        /// <param name="HWtype"></param>
        private static void ConfigureHWDecoder(out AVHWDeviceType HWtype)
        {
            HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
            Console.WriteLine("Use hardware acceleration for decoding?[n]");
            var key = Console.ReadLine();
            var availableHWDecoders = new Dictionary<int, AVHWDeviceType>();

            if (key == "y")
            {
                Console.WriteLine("Select hardware decoder:");
                var type = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
                var number = 0;

                while ((type = ffmpeg.av_hwdevice_iterate_types(type)) != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
                {
                    Console.WriteLine($"{++number}. {type}");
                    availableHWDecoders.Add(number, type);
                }

                if (availableHWDecoders.Count == 0)
                {
                    Console.WriteLine("Your system have no hardware decoders.");
                    HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
                    return;
                }

                var decoderNumber = availableHWDecoders
                    .SingleOrDefault(t => t.Value == AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2).Key;
                if (decoderNumber == 0)
                    decoderNumber = availableHWDecoders.First().Key;
                Console.WriteLine($"Selected [{decoderNumber}]");
                int.TryParse(Console.ReadLine(), out var inputDecoderNumber);
                availableHWDecoders.TryGetValue(inputDecoderNumber == 0 ? decoderNumber : inputDecoderNumber,
                    out HWtype);
            }
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="HWDevice">硬件类型</param>
        private static unsafe void DecodeAllFramesToImages(AVHWDeviceType HWDevice)
        {
            // decode all frames from url, please not it might local resorce, e.g. string url = "../../sample_mpeg4.mp4";
            var url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"; // be advised this file holds 1440 frames
            //使用自行编写的视频解码器进行编码
            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {

                Console.WriteLine($"codec name: {vsd.CodecName}");
                
                //获取媒体信息
                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                //资源编码格式
                var sourcePixelFormat = HWDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE
                    ? vsd.PixelFormat
                    : GetHWPixelFormat(HWDevice);
                //目标尺寸与原尺寸一致
                var destinationSize = sourceSize;
                //目标媒体格式是Bit类型
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                //帧格式转换
                using (var vfc =
                    new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {

                    var frameNumber = 0;

                    while (vsd.TryDecodeNextFrame(out var frame))
                    {
                        var convertedFrame = vfc.Convert(frame);

                        using (var bitmap = new Bitmap(convertedFrame.width,
                            convertedFrame.height,
                            convertedFrame.linesize[0],
                            System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                            (IntPtr)convertedFrame.data[0]))
                            bitmap.Save($"frame.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        Console.WriteLine($"frame: {frameNumber}");
                        frameNumber++;
                    }
                }
            }
        }

        /// <summary>
        /// 根据硬件类型返回像素类型
        /// </summary>
        /// <param name="hWDevice">硬件枚举</param>
        /// <returns>像素枚举</returns>
        private static AVPixelFormat GetHWPixelFormat(AVHWDeviceType hWDevice)
        {
            AVPixelFormat avPixelFormat;
            switch (hWDevice)
            {
                case AVHWDeviceType.AV_HWDEVICE_TYPE_NONE:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_VDPAU;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_CUDA;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_VAAPI;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_NV12;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_QSV:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_QSV;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_NV12;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DRM:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_DRM_PRIME;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_OPENCL;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_MEDIACODEC;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VULKAN:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_VULKAN;
                    break;
                default:
                    avPixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
                    break;
            }
            return avPixelFormat;
        }

        private static unsafe void EncodeImagesToH264()
        {
            var frameFiles = Directory.GetFiles(".", "frame.*.jpg").OrderBy(x => x).ToArray();
            var fistFrameImage = System.Drawing.Image.FromFile(frameFiles.First());

            var outputFileName = "out.h264";
            var fps = 25;
            var sourceSize = fistFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
            var destinationSize = sourceSize;
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (var vfc =
                new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {

                using (var fs = File.Open(outputFileName, FileMode.Create))
                {

                    using (var vse = new H264VideoStreamEncoder(fs, fps, destinationSize))
                    { 
                        var frameNumber = 0;

                        foreach (var frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (var frameImage = System.Drawing.Image.FromFile(frameFile))
                            {
                                using (var frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                                {
                                    bitmapData = GetBitmapData(frameBitmap);

                                    fixed (byte* pBitmapData = bitmapData)
                                    {
                                        var data = new byte_ptrArray8 { [0] = pBitmapData };
                                        var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                        var frame = new AVFrame
                                        {
                                            data = data,
                                            linesize = linesize,
                                            height = sourceSize.Height
                                        };
                                        var convertedFrame = vfc.Convert(frame);
                                        convertedFrame.pts = frameNumber * fps;
                                        vse.Encode(convertedFrame);
                                    }
                                }
                            }
                        }
                        Console.WriteLine($"frame: {frameNumber}");
                        frameNumber++;
                    }
                }
            }
        }

        private static byte[] GetBitmapData(Bitmap frameBitmap)
        {
            var bitmapData = frameBitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, frameBitmap.Size),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                var data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data, 0, length);
                return data;
            }
            finally
            {
                frameBitmap.UnlockBits(bitmapData);
            }
        }
        #endregion
    }
}
