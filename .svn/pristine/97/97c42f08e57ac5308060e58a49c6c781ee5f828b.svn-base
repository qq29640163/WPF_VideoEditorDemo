using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.ViewModel.DialogViewModel
{
    public class BlurDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;


        private string _width = "10";

        public string Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private string _height = "10";

        public string Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private string _x = "10";

        public string X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }

        private string _y = "10";

        public string Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        private string _blur="10";

        public string Blur
        {
            get { return _blur; }
            set { SetProperty(ref _blur, value); }
        }

        public DelegateCommand SubmitCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public BlurDialogViewModel()
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
            
        }

        private void OnSubmit()
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Yes);
            dialogResult.Parameters.Add("BlurParameter", new object[] { Height, Width, X, Y, "boxblur" ,Blur });
            RequestClose.Invoke(dialogResult);
        }
        private void OnCancel() 
        {
            DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);
            RequestClose.Invoke(dialogResult);
        }
    }
}
