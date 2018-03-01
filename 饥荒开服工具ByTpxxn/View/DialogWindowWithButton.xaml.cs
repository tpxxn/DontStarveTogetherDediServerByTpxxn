using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using 饥荒开服工具ByTpxxn.Class;

namespace 饥荒开服工具ByTpxxn.View
{
    /// <summary>
    /// CopySplashScreen.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindowWithButton : Window
    {
        public object Result;
        public enum DialogButtons
        {
            OK,
            Cancel,
            OKCancel
        }

        public DialogWindowWithButton(string content,DialogButtons dialogButtons)
        {
            InitializeComponent();
            ContentTextBlock.Text = content;
            if (Global.FontFamily != null)
                FontFamily = Global.FontFamily;
            switch (dialogButtons)
            {
                case DialogButtons.OK:
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case DialogButtons.Cancel:
                    OKButton.Visibility = Visibility.Collapsed;
                    break;
                case DialogButtons.OKCancel:
                    break;
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OKbuttonEvent?.Invoke(this);
            Close();
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// SelectChanged事件委托
        /// </summary>
        public delegate void OKbuttonEventHandler(DialogWindowWithButton dialogWindowWithButton);


        /// <summary>
        /// SelectChanged事件
        /// </summary>
        public event OKbuttonEventHandler OKbuttonEvent;

    }
}
