using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ZPC.Phone.Model;

namespace ZPC.Phone.ViewModel.DialogViewModel
{
    class WatermarkTextDialogViewModel : BindableBase, IDialogAware
    {

        #region Command
        public DelegateCommand CancelCommand { get; set; }

        public DelegateCommand SubmitCommand { get; set; }
        #endregion

        #region NotifyProperty

        public ObservableCollection<int> FontSizes { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<string> Fonts { get; set; } = new ObservableCollection<string>();

        private string _font;

        public string Font
        {
            get { return _font; }
            set { SetProperty(ref _font, value); }
        }

        private int _fontSize;

        public int FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value); }
        }


        private SolidColorBrush _selectColor = new SolidColorBrush(Colors.Red);

        public SolidColorBrush SelectColor
        {
            get { return _selectColor; }
            set { SetProperty(ref _selectColor, value); }
        }

        private string _txtContent;

        public string TxtContent
        {
            get { return _txtContent; }
            set { SetProperty(ref _txtContent, value); }
        }

        private int _x;

        public int X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }


        private int _y;

        public int Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        private int start_HH;

        public int Start_HH
        {
            get { return start_HH; }
            set { SetProperty(ref start_HH, value); }
        }

        private int start_mm;

        public int Start_mm
        {
            get { return start_mm; }
            set { SetProperty(ref start_mm, value); }
        }

        private int start_ss;

        public int Start_ss
        {
            get { return start_ss; }
            set { SetProperty(ref start_ss, value); }
        }

        private int end_HH;

        public int End_HH
        {
            get { return end_HH; }
            set { SetProperty(ref end_HH, value); }
        }

        private int end_mm;

        public int End_mm
        {
            get { return end_mm; }
            set { SetProperty(ref end_mm, value); }
        }

        private int end_ss;

        public int End_ss
        {
            get { return end_ss; }
            set { SetProperty(ref end_ss, value); }
        }

        #endregion

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        public WatermarkTextDialogViewModel()
        {
            SubmitCommand = new DelegateCommand(OnSubmit);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
         
            foreach (System.Drawing.FontFamily family in fonts.Families)
            {
                Fonts.Add(family.Name);
            };
            Font = Fonts.First();

            for (int i = 10; i <=150; i++)
            {
                FontSizes.Add(i);
            }
            FontSize = FontSizes[2];
        }

        /// <summary>
        /// 根据字体名称获取字体的文件路径
        /// </summary>
        /// <param name="fontName">字体名称</param>
        /// <returns></returns>
        private string GetFontFileName(string fontName)
        {
            string folderFullName = System.Environment.GetEnvironmentVariable("windir") + "\\fonts";
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            foreach (var file in TheFolder.GetFiles())
            {
                if (file.Exists)
                {
                    if (fontName == GetFontName(file.FullName))
                        return file.Name;
                }
            }
            //找不到对应字体文件默认返回
            return "arial.ttf";
        }

        private string GetFontName(string fontfilename)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            if (fontfilename.EndsWith(".ttf") || fontfilename.EndsWith(".TTF"))
                pfc.AddFontFile(fontfilename);
            if (pfc.Families.Length > 0)
                return pfc.Families[0].Name;
            return "";
        }

        private void OnSubmit()
        {
            if (string.IsNullOrEmpty(TxtContent))
            {
                MessageBox.Show("请设置文字内容！");
                return;
            }

            Watermark watermark = new Watermark();
            watermark.Text = TxtContent;
            watermark.FontSize = FontSize;
            watermark.WatermarkLocation = new Point(X, Y);
            watermark.FontFamily = GetFontFileName(Font);
            watermark.FontColor = SelectColor.Color.ToString();
            watermark.IsImage = false;
            watermark.StartTime = new TimeSpan(Start_HH, Start_mm, Start_ss);
            watermark.EndTime = new TimeSpan(End_HH, End_mm, End_ss);
            if (watermark.StartTime.TotalSeconds >= watermark.EndTime.TotalSeconds)
            {
                System.Windows.Forms.MessageBox.Show("持续开始时间必须小于结束时间");
                return;
            }

            StringBuilder sb = new StringBuilder();
            string A = watermark.FontColor.Substring(1, 2);
            string RGB = watermark.FontColor.Substring(3, watermark.FontColor.Length - 3);
            sb.Append(RGB);
            sb.Append(A);

            watermark.FontColor = "#" + sb.ToString();

            DialogResult dialogResult = new DialogResult(ButtonResult.Yes);
            dialogResult.Parameters.Add("Watermark", watermark);
            RequestClose.Invoke(dialogResult);
        }

        private void OnCancel()
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(dialogResult);
        }
    }
}