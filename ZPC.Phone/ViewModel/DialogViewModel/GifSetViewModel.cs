using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.ViewModel.DialogViewModel
{
    public class GifSetViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        private string _width;

        public string Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private string _height;

        public string Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private int _frameRate;

        public int FrameRate
        {
            get { return _frameRate; }
            set { SetProperty(ref _frameRate, value); }
        }

        private string _start;

        public string Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }

        private string _end;

        public string End
        {
            get { return _end; }
            set { SetProperty(ref _end, value); }
        }

        public DelegateCommand SubmitCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public ObservableCollection<int> FrameRates { get; set; } = new ObservableCollection<int>();

        public GifSetViewModel()
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
            //
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            FrameRates.Add(15);
            FrameRates.Add(24);
            FrameRates.Add(30);
            FrameRate = FrameRates[0];
            Start = "00:00:00";
            End = "00:00:01";
            Width = "1280";
            Height = "720";
        }

        private void OnSubmit()
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Yes);
            dialogResult.Parameters.Add("GifParameter", new object[] { Start, End, Width, Height, FrameRate });
            RequestClose.Invoke(dialogResult);
        }

        private void OnCancel()
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(dialogResult);
        }
    }
}
