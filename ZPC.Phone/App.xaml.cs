using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZPC.Phone.Model;

namespace ZPC.Phone
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App 
    {
        /// <summary>
        /// prism框架程序起始
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        #region 加载登录界面,业务模块
        /// <summary>
        /// 打开起始页
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// 依赖注入，注册绑定第一个执行的view和viewmodel
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            base.ConfigureViewModelLocator();
        }

        /// <summary>
        /// 创建模块目录,
        /// 扫描目录的内容，定位类实现Prism.Modularity.IModule并基于相关Prism.Modularity.ModuleAttribute中的内容。
        /// 组件是使用ReflectionOnlyLoad加载到新的应用程序域。应用程序一旦发现程序集，域将被销毁。
        /// 目录创建初始值后，不会继续监视目录
        /// </summary>
        /// <returns></returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            //return base.CreateModuleCatalog();
            string modulePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model");
            Directory.CreateDirectory(modulePath);
            return new DirectoryModuleCatalog { ModulePath = modulePath };
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterInstance<IAppleDeviceMonitor>(new AppleDeviceMonitorService());
            //containerRegistry.RegisterInstance<IByPassService>(new ByPassService());
            new ModuleContainer().RegisterTypes(containerRegistry);
        }

        #endregion
    }
}
