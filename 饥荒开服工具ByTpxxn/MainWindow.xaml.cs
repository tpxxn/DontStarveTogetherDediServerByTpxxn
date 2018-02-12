using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.View;

namespace 饥荒开服工具ByTpxxn
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 字段/属性

        #region "窗口可视化属性"
        private readonly System.Windows.Forms.Timer _visiTimer = new System.Windows.Forms.Timer();

        public bool MwVisibility { get; set; }

        private void VisiTimerEvent(object sender, EventArgs e)
        {
            Visibility = MwVisibility ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        /// <summary>
        /// 窗口是否初始化
        /// </summary>
        public static bool MwInit { get; set; }

        /// <summary>
        /// 是否加载字体
        /// </summary>
        public static bool LoadFont;
        #endregion

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            #region "读取注册表(必须在初始化之前读取)"
            // 背景图片
            var bg = IniFileIo.IniFileReadString("Configure", "Skin", "Background");
            var bgStretch = IniFileIo.IniFileReadDouble("Configure", "Skin", "BackgroundStretch");
            // 透明度
            var bgAlpha = IniFileIo.IniFileReadDouble("Configure", "Skin", "BackgroundAlpha");
            //var bgPanelAlpha = RegeditRw.RegRead("BGPanelAlpha");
            var windowAlpha = IniFileIo.IniFileReadDouble("Configure", "Skin", "WindowAlpha");
            // 窗口大小
            var mainWindowHeight = IniFileIo.IniFileReadDouble("Configure", "Window", "Height");
            var mainWindowWidth = IniFileIo.IniFileReadDouble("Configure", "Window", "Width");
            // 字体
            var mainWindowFont = IniFileIo.IniFileReadString("Configure", "Font", "FontFamily");
            var mainWindowFontWeight = IniFileIo.IniFileReadString("Configure", "Font", "FontWeight");
            // 淡紫色透明光标
            var mainWindowLavenderCursor = IniFileIo.IniFileReadString("Configure", "Others", "LavenderCursor");
            // 设置菜单
            var winTopmost = IniFileIo.IniFileReadDouble("Configure", "Others", "Topmost");
            // 设置
            //Settings.HideToNotifyIcon = RegeditRw.RegReadString("HideToNotifyIcon") == "True";
            //Settings.HideToNotifyIconPrompt = RegeditRw.RegReadString("HideToNotifyIconPrompt") == "True";
            //Settings.SmallButtonMode = RegeditRw.RegReadString("SmallButtonMode") == "True";
            #endregion
            // 初始化
            InitializeComponent();
            // 窗口缩放
            SourceInitialized += delegate (object sender, EventArgs e) { _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource; };
            MouseMove += Window_MouseMove;
            // mainWindow初始化标志
            MwInit = true;
            #region "读取设置"
            // 设置字体
            if (string.IsNullOrEmpty(mainWindowFont))
            {
                IniFileIo.IniFileWrite("Configure", "Font", "FontFamily", "微软雅黑");
                mainWindowFont = "微软雅黑";
            }
            mainWindow.FontFamily = new FontFamily(mainWindowFont);
            ((TextBlock)((VisualBrush)FindResource("HelpBrush")).Visual).FontFamily = mainWindow.FontFamily;
            // 设置字体加粗
            if (string.IsNullOrEmpty(mainWindowFontWeight))
            {
                IniFileIo.IniFileWrite("Configure", "Font", "FontWeight", "False");
            }
            mainWindow.FontWeight = mainWindowFontWeight == "True" ? FontWeights.Bold : FontWeights.Normal;
            ((TextBlock)((VisualBrush)FindResource("HelpBrush")).Visual).FontWeight = mainWindow.FontWeight;
            Global.FontWeight = mainWindow.FontWeight;
            // 设置淡紫色透明光标
            SeCheckBoxLavenderCursor.IsChecked = mainWindowLavenderCursor == "True";
            // 版本初始化
            UiTitle.Text = "饥荒开服工具" + Assembly.GetExecutingAssembly().GetName().Version;
            // 窗口可视化计时器
            _visiTimer.Enabled = true;
            _visiTimer.Interval = 200;
            _visiTimer.Tick += VisiTimerEvent;
            _visiTimer.Start();
            // 设置光标资源字典路径
            if (SeCheckBoxLavenderCursor.IsChecked == true)
            {
                CursorDictionary.Source =
                    new Uri("pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/Cursor/CursorDictionary.xaml",
                        UriKind.Absolute);
            }
            else
            {
                CursorDictionary.Source =
                    new Uri("pack://application:,,,/饥荒开服工具ByTpxxn;component/Dictionary/Cursor/DefaultCursorDictionary.xaml",
                        UriKind.Absolute);
            }
            // 显示窗口
            MwVisibility = true;
            // 窗口置顶
            if (winTopmost == 1)
            {
                Se_button_Topmost_Click(null, null);
            }
            // 设置背景
            if (bg == "")
            {
                Se_button_Background_Clear_Click(null, null);
            }
            else
            {
                try
                {
                    var brush = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(bg))
                    };
                    UiBackGroundBorder.Background = brush;
                    UiTitle.Foreground = new SolidColorBrush(Colors.Black);
                }
                catch
                {
                    Se_button_Background_Clear_Click(null, null);
                }
            }
            // 设置背景拉伸方式
            if (bgStretch == 0)
            {
                bgStretch = 2;
            }
            SeComboBoxBackgroundStretch.SelectedIndex = (int)bgStretch - 1;
            // 设置背景透明度
            if (bgAlpha == 0)
            {
                bgAlpha = 101;
            }
            SeBgAlpha.Value = bgAlpha - 1;
            SeBgAlphaText.Text = "背景不透明度：" + (int)SeBgAlpha.Value + "%";
            UiBackGroundBorder.Opacity = (bgAlpha - 1) / 100;
            // 设置面板透明度
            //if (bgPanelAlpha == 0)
            //{
            //    bgPanelAlpha = 61;
            //}
            //SePanelAlpha.Value = bgPanelAlpha - 1;
            //SePanelAlphaText.Text = "面板不透明度：" + (int)SePanelAlpha.Value + "%";
            // 设置窗口透明度
            if (windowAlpha == 0)
            {
                windowAlpha = 101;
            }
            SeWindowAlpha.Value = windowAlpha - 1;
            SeWindowAlphaText.Text = "窗口不透明度：" + (int)SeWindowAlpha.Value + "%";
            Opacity = (windowAlpha - 1) / 100;
            // 设置高度和宽度
            Width = mainWindowWidth;
            Height = mainWindowHeight;
            #endregion
            // 设置托盘区图标
            //SetNotifyIcon();
            // Frame导航
            DedicatedServerFrame.Navigate(new DedicatedServerPage());
            // 是否显示开服工具
            //if (Global.TestMode)
            //    SidebarDedicatedServer.Visibility = Visibility.Visible;
            // 检测新版本
            //UpdatePan.UpdateNow();
        }

        /// <summary>
        /// MainWindow窗口加载
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region 加载字体
            foreach (var str in ReadFont())
            {
                var textBlock = new TextBlock
                {
                    Text = str,
                    FontFamily = new FontFamily(str)
                };
                SeComboBoxFont.Items.Add(textBlock);
            }
            var mainWindowFont = IniFileIo.IniFileReadString("Configure", "Font", "FontFamily");
            var stringList = (from TextBlock textBlock in SeComboBoxFont.Items select textBlock.Text).ToList();
            SeComboBoxFont.SelectedIndex = stringList.IndexOf(mainWindowFont);
            var mainWindowFontWeight = IniFileIo.IniFileReadString("Configure", "Font", "FontWeight");
            SeCheckBoxFontWeight.IsChecked = mainWindowFontWeight == "True";
            LoadFont = true;
            #endregion
        }

    }
}
