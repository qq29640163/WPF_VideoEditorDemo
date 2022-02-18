//using iMobie.Miniblink;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;

namespace ZPC.Phone.ViewModel
{
    public class MiniblinkViewModel : BindableBase, INavigationAware
    {
        //private MiniblinkBrowser miniblinkBrowser;

        private WindowsFormsHost _windowsFormsHost = new WindowsFormsHost() { FlowDirection = FlowDirection.LeftToRight };

        public DelegateCommand ClickCommand { get; set; }

        private string _textURL;
        public string TextURL
        {
            get { return _textURL; }
            set { SetProperty(ref _textURL, value); }
        }

        public WindowsFormsHost WindowsFormsHosts
        {
            get { return _windowsFormsHost; }
            set { SetProperty(ref _windowsFormsHost, value); }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //miniblinkBrowser = new MiniblinkBrowser();
            //using (Graphics gs = Graphics.FromHwnd(miniblinkBrowser.Handle))
            //{
            //    //计算屏幕XY轴的缩放比例
            //    float dpiX = gs.DpiX / 96;
            //    float dpiY = gs.DpiY / 96;
            //    miniblinkBrowser.Zoom = dpiX;
            //}
            //WindowsFormsHosts.Child = miniblinkBrowser;

            //ClickCommand = new DelegateCommand(() =>
            //{
            //    miniblinkBrowser.LoadUri(TextURL);
            //});

        }
    }
}
