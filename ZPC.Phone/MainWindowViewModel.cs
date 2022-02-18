using CommonServiceLocator;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ZPC.Phone.Command;
using ZPC.Phone.Enum;

namespace ZPC.Phone
{
    public class MainWindowViewModel: BindableBase, INavigationAware, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private List<Model.MenuItem> menulist;
        public List<Model.MenuItem> MenuList
        {
            get { return menulist; }
            set
            {
                menulist = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(MenuList)));
                    //或者用Prism框架的 SetProperty()方法
                }
            }
        }
        
        private object _selectValue;
        public object SelectValue
        {
            get { return _selectValue; }
            set 
            { 
                SetProperty(ref _selectValue, value, SelectValueChange);
            }
        }

        private void SelectValueChange()
        {
            var Item = SelectValue as Model.MenuItem;
            switch (Item.Name)
            {
                case "设备信息":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.DeviceInfo.ToString());
                    break;
                case "文件管理":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.FileManage.ToString());
                    break;
                case "画板":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.DrawingBoard.ToString());
                    break;
                case "Miniblink浏览器":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.Miniblink.ToString());
                    break;
                case "视频编辑":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.VideoEdit.ToString());
                    break;
                case "摄像头编辑":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.CameraEdit.ToString());
                    break;
                case "其他":
                    ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("Main_Shell", NavigateName.Other.ToString());
                    break;
            }

        }

        /// <summary>
        /// 使用原生接口形式实现命令,窗口最小化
        /// </summary>
        public ICommand WindowMinSizeCommand { get; set; }
        public ICommand CommandLoaded { get; set; }

        /// <summary>
        /// 使用prism框架命令实现窗口关闭
        /// </summary>
        public DelegateCommand<object> WindowCloseCommand { get; set; }

        public MainWindowViewModel()
        {
            //CommandLoaded = new CommandBase(new Action<object>((e) =>
            //{

            //    //SelectValue = MenuList[1];
            //}));
            WindowMinSizeCommand = new CommandBase(new Action<object>((e)=> 
            {
                (e as MainWindow).WindowState = System.Windows.WindowState.Minimized;
            }));
            (WindowMinSizeCommand as CommandBase).DoCanExecute= new Func<object, bool>((e)=> { return true; });

            WindowCloseCommand = new DelegateCommand<object>((o) => 
            {
                (o as MainWindow).Close();
            });
            MenuList = new List<Model.MenuItem>();
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "设备信息";
                MenuList.Add(item);
            }
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "视频编辑";
                MenuList.Add(item);
            }
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "文件管理";
                MenuList.Add(item);
            }

            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "画板";
                MenuList.Add(item);
            }
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "Miniblink浏览器";
                MenuList.Add(item);
            }
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "摄像头编辑";
                MenuList.Add(item);
            }
            using (Model.MenuItem item = new Model.MenuItem())
            {
                item.Name = "其他";
                MenuList.Add(item);
            }
            

        }


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //调用以确定此实例是否可以处理导航请求。
            //如果此实例接受导航请求，则为true；否则，false。
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
