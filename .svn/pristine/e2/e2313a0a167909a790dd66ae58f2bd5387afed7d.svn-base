using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZPC.Phone.Enum;
using ZPC.Phone.View;
using ZPC.Phone.View.DialogView;
using ZPC.Phone.ViewModel;
using ZPC.Phone.ViewModel.DialogViewModel;

namespace ZPC.Phone.Model
{
    public class ModuleContainer : IModule
    {
        IContainerRegistry _containerRegistry;
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _containerRegistry = containerRegistry;
            OnRegisterTypes();
        }

        /// <summary>
        /// 注册页面
        /// </summary>
        public void OnRegisterTypes()
        {
            RegisterDialog<WatermarkDialogView, WatermarkDialogViewModel>(DialogName.WatermarkSetting);
            RegisterDialog<WatermarkTextDialogView, WatermarkTextDialogViewModel>(DialogName.WatermarkTextSetting);
            RegisterDialog<GifSetView, GifSetViewModel>(DialogName.GifSet);
            RegisterDialog<BlurDialogView, BlurDialogViewModel>(DialogName.Blur);

            RegisterForNavigation<DeviceInfoView, DeviceInfoViewModel>(NavigateName.DeviceInfo);
            RegisterForNavigation<FileManageView, FileManageViewModel>(NavigateName.FileManage);
            RegisterForNavigation<DrawingBoardView, DrawingBoardViewModel>(NavigateName.DrawingBoard);
            RegisterForNavigation<OtherView, OtherViewModel>(NavigateName.Other);
            RegisterForNavigation<MiniblinkView, MiniblinkViewModel>(NavigateName.Miniblink);
            RegisterForNavigation<VideoEditView, VideoEditViewModel>(NavigateName.VideoEdit);
            RegisterForNavigation<CameraEditView, CameraEditViewModel>(NavigateName.CameraEdit);
        }

        #region 注册
        /// <summary>
        /// 设置弹出窗口的Window样式，只需注册一次
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        public void RegisterDialogWindow<TWindow>()
            where TWindow : IDialogWindow
        {
            _containerRegistry.RegisterDialogWindow<TWindow>();
        }

        public void RegisterDialog<TView>(DialogName dialogWindowName)
        {
            _containerRegistry.RegisterDialog<TView>(dialogWindowName.ToString());
        }

        public void RegisterDialog<TView, TViewModel>(DialogName dialogWindowName) where TViewModel : IDialogAware
        {
            ViewModelLocationProvider.Register<TView, TViewModel>();
            _containerRegistry.RegisterDialog<TView, TViewModel>(dialogWindowName.ToString());
        }


        public void RegisterForNavigation(Type view, NavigateName navigativeName)
        {
            _containerRegistry.RegisterForNavigation(view, navigativeName.ToString());
        }

        public void RegisterForNavigation<TView>(NavigateName navigativeName)
        {
            _containerRegistry.RegisterForNavigation<TView>(navigativeName.ToString());
        }

        public void RegisterForNavigation<TView, TViewModel>(NavigateName navigativeName)
        {
            ViewModelLocationProvider.Register<TView, TViewModel>();
            _containerRegistry.RegisterForNavigation<TView, TViewModel>(navigativeName.ToString());
        }

        public void RegisterSingleton(Type from)
        {
            _containerRegistry.RegisterSingleton(from);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            _containerRegistry.RegisterSingleton(from, to);
        }

        public bool IsRegistered(Type from)
        {
            return _containerRegistry.IsRegistered(from);
        }

        public bool IsRegistered(Type from, string name)
        {
            return _containerRegistry.IsRegistered(from, name);
        }

        public bool IsRegistered<T>()
        {
            return _containerRegistry.IsRegistered<T>();
        }

        public bool IsRegistered<T>(string name)
        {
            return _containerRegistry.IsRegistered<T>(name);
        }

        #endregion 注册
    }
}
