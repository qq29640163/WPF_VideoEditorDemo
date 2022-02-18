using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZPC.Phone.View
{
    /// <summary>
    /// VideoEditView.xaml 的交互逻辑
    /// </summary>
    public partial class VideoEditView : UserControl
    {
        Window window;
        public VideoEditView()
        {
            InitializeComponent();
            a.Opened += A_Opened;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            log.ScrollToEnd();
        }

        private void a_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(a);
            window.LocationChanged += Window_LocationChanged;
        }

        /// <summary>
        /// 必须在Opened事件里去处理popu的绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void A_Opened(object sender, EventArgs e)
        {
            //绘制pop处于用户界面的最底层
            var hwnd = ((HwndSource)PresentationSource.FromVisual(a)).Handle;
            RECT rect;
            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)a.Width, (int)a.Height, 1);
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var offset = a.HorizontalOffset;
            a.HorizontalOffset = offset + 1;
            a.HorizontalOffset = offset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    }
}
