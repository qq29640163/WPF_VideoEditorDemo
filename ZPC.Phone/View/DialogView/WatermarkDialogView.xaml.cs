using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZPC.Phone.View.DialogView
{
    /// <summary>
    /// WatermarkDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WatermarkDialogView : UserControl
    {
        public WatermarkDialogView()
        {
            InitializeComponent();
        }

        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
            image.Opacity = 0.5;
            //(Brush)new BrushConverter().ConvertFromString($"#d1d6db");
        }

        private void image_MouseLeave(object sender, MouseEventArgs e)
        {
            image.Opacity = 1;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);

            if (e.Handled)
                return;
        }

        private void TextBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
            if (e.Handled)
                return;

            TextBox txtBox = sender as TextBox;
            string str=string.Empty;
            if (txtBox.Text!="0")
            {
                str = txtBox.Text + e.Text;
            }
            if (!string.IsNullOrEmpty(str))
            {
                int value = int.Parse(str);
                if (value > 59)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void TextBox_PreviewTextInput_2(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
            if (e.Handled)
                return;
            TextBox txtBox = sender as TextBox;
            string str = string.Empty;
            if (txtBox.Text != "0")
            {
                str = txtBox.Text + e.Text;
            }
            if (!string.IsNullOrEmpty(str))
            {
                int value = int.Parse(str);
                if (value > 59)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
