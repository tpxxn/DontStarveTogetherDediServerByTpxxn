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
    public partial class DialogWindow : Window
    {
        private readonly Timer _dialogWindowTimer = new Timer();

        public DialogWindow(string content)
        {
            InitializeComponent();
            ContentTextBlock.Text = content;
            if (Global.FontFamily != null)
                FontFamily = Global.FontFamily;
            _dialogWindowTimer.Interval = 1;
            _dialogWindowTimer.Tick += DialogWindowTick;
            _dialogWindowTimer.Start();
        }

        private void DialogWindowTick(object sender, EventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            // ReSharper disable once PossibleNullReferenceException
            Left = mainWindow.Left + mainWindow.Width / 2 - Width / 2;
            Top = mainWindow.Top + mainWindow.Height / 2 - Height / 2;
        }
    }
}
