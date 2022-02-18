using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZPC.Phone.Model;

namespace ZPC.Phone.ViewModel.DialogViewModel
{
    public class WatermarkDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;


        #region NotifyProperty
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

        private int _width;

        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private ImageSource _watermarkSource = System.Windows.Application.Current.FindResource("AdditionDrawingImage") as ImageSource;

        public ImageSource WatermarkSource
        {
            get { return _watermarkSource; }
            set { SetProperty(ref _watermarkSource, value); }
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

        #region Command
        public DelegateCommand AddWatermarkCommand { get; set; }
        public DelegateCommand SubmitCommand { get; set; }

        public DelegateCommand CancelCommand { get; set; }
        #endregion

        private Watermark watermark = new Watermark();

        public WatermarkDialogViewModel()
        {
            AddWatermarkCommand = new DelegateCommand(OnAddWatermark);
            SubmitCommand = new DelegateCommand(OnSubmit);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        #region Event

        private void OnAddWatermark() 
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "选择水印图片";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "*|*.jpg;*.png;*.jpeg;*.bmp";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                watermark.ImagePath = openFileDialog.FileName;
                watermark.ImageWatermark=new BitmapImage(new Uri(watermark.ImagePath));
                WatermarkSource = watermark.ImageWatermark;
            }
            //Bitmap bitmap = new Bitmap(watermark.ImagePath);
            //BitmapData bitmapData = bitmap.LockBits(Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //BitmapSource image = BitmapSource.Create(bitmap.Width, bitmap.Height, 96, 96, PixelFormats.Pbgra32, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            //bitmap.UnlockBits(bitmapData);
        }

        private void OnSubmit()
        {
            if(string.IsNullOrEmpty(watermark.ImagePath))
            {

                System.Windows.Forms.MessageBox.Show("请设置水印图片");
                return;
            }
            watermark.ImageSize = new System.Windows.Size(Width, Height);
            watermark.WatermarkLocation = new System.Windows.Point(X, Y);
            watermark.IsImage = true;
            watermark.StartTime = new TimeSpan(Start_HH, Start_mm, Start_ss);
            watermark.EndTime = new TimeSpan(End_HH, End_mm, End_ss);
            if (watermark.StartTime.TotalSeconds >= watermark.EndTime.TotalSeconds)
            {
                System.Windows.Forms.MessageBox.Show("持续开始时间必须小于结束时间");
                return;
            }
            DialogResult dialogResult = new DialogResult(ButtonResult.Yes);

            if (watermark.StartTime == TimeSpan.Zero) 
                return;
            dialogResult.Parameters.Add("Watermark", watermark);
            RequestClose.Invoke(dialogResult);
        }

        private void OnCancel() 
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(dialogResult);
        }

        #endregion

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
