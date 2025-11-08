using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Velopack;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SSMT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            
            VelopackApp.Build().Run();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            // 注册 Toast 激活处理器
            AppNotificationManager.Default.NotificationInvoked += OnToastActivated;
            AppNotificationManager.Default.Register();
        }

        //必须设为public static 这样非打包的WinUI3程序的Page里才能获取主窗口句柄来实现调用显示其它窗口
        public static Window m_window { get; set; }



        private void OnToastActivated(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // 处理 Toast 点击逻辑
            if (args.Arguments.TryGetValue("action", out string action))
            {
                if (action == "viewDetails")
                {
                    // 执行相应操作
                }
            }
        }

       

    }
}
