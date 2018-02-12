using System.Drawing.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Properties;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace 饥荒开服工具ByTpxxn
{
    public partial class MainWindow : Window
    {
        #region "窗口尺寸/拖动窗口"
        // 引用光标资源字典
        private static readonly ResourceDictionary CursorDictionary = new ResourceDictionary();
        private const int WmSyscommand = 0x112;
        private HwndSource _hwndSource;
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) return;
            if (e.OriginalSource is FrameworkElement element && !element.Name.Contains("Resize"))
            {
                Cursor = ((TextBlock)CursorDictionary["CursorPointer"]).Cursor;
            }
        }

        private void ResizePressed(object sender, MouseEventArgs e)
        {
            if (!(sender is FrameworkElement element)) return;
            var direction = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), element.Name.Replace("Resize", ""));

            switch (direction)
            {
                case ResizeDirection.Left:
                    Cursor = ((TextBlock)CursorDictionary["CursorHorz"]).Cursor;
                    break;
                case ResizeDirection.Right:
                    Cursor = ((TextBlock)CursorDictionary["CursorHorz"]).Cursor;
                    break;
                case ResizeDirection.Top:
                    Cursor = ((TextBlock)CursorDictionary["CursorVert"]).Cursor;
                    break;
                case ResizeDirection.Bottom:
                    Cursor = ((TextBlock)CursorDictionary["CursorVert"]).Cursor;
                    break;
                case ResizeDirection.TopLeft:
                    Cursor = ((TextBlock)CursorDictionary["CursorDgn1"]).Cursor;
                    break;
                case ResizeDirection.BottomRight:
                    Cursor = ((TextBlock)CursorDictionary["CursorDgn1"]).Cursor;
                    break;
                case ResizeDirection.TopRight:
                    Cursor = ((TextBlock)CursorDictionary["CursorDgn2"]).Cursor;
                    break;
                case ResizeDirection.BottomLeft:
                    Cursor = ((TextBlock)CursorDictionary["CursorDgn2"]).Cursor;
                    break;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
                ResizeWindow(direction);
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, WmSyscommand, (IntPtr)(61440 + direction), IntPtr.Zero);
            var systemWorkAreaRect = SystemParameters.WorkArea;
            if (Width == systemWorkAreaRect.Width && Height == systemWorkAreaRect.Height)
            {
                UiBtnMaximized.Visibility = Visibility.Collapsed;
                UiBtnNormal.Visibility = Visibility.Visible;
            }
            else
            {
                UiBtnMaximized.Visibility = Visibility.Visible;
                UiBtnNormal.Visibility = Visibility.Collapsed;
            }
            var dictionary = new Dictionary<string, string>
            {
                {"Width", ActualWidth.ToString(CultureInfo.InvariantCulture)},
                {"Height", ActualHeight.ToString(CultureInfo.InvariantCulture)}
            };
            IniFileIo.IniFileWrite("Configure", "Window", dictionary);
        }

        /// <summary>
        /// MainWindow拖动窗口
        /// </summary>
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var positionUiGrid = e.GetPosition(UiGrid);
            var positionDedicatedServerFrame = e.GetPosition(DedicatedServerFrame);
            var inUiGrid = false;
            var inDedicatedServerFrame = false;
            if (positionUiGrid.X >= 0 && positionUiGrid.X < UiGrid.ActualWidth && positionUiGrid.Y >= 0 && positionUiGrid.Y < UiGrid.ActualHeight)
            {
                inUiGrid = true;
            }
            if (positionDedicatedServerFrame.X >= 0 && positionDedicatedServerFrame.X < DedicatedServerFrame.ActualWidth && positionDedicatedServerFrame.Y >= 0 && positionDedicatedServerFrame.Y < DedicatedServerFrame.ActualHeight)
            {
                inDedicatedServerFrame = true;
            }
            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton != MouseButtonState.Pressed || (!inUiGrid && !inDedicatedServerFrame)) return;
            Cursor = ((TextBlock)CursorDictionary["CursorMove"]).Cursor;
            DragMove();
        }

        /// <summary>
        /// 切换鼠标指针为默认状态
        /// </summary>
        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = ((TextBlock)CursorDictionary["CursorPointer"]).Cursor;
            var dictionary = new Dictionary<string, string>
                {
                    {"Width", ActualWidth.ToString(CultureInfo.InvariantCulture)},
                    {"Height", ActualHeight.ToString(CultureInfo.InvariantCulture)}
                };
            IniFileIo.IniFileWrite("Configure", "Window", dictionary);
        }

        /// <summary>
        /// 双击标题栏最大化
        /// </summary>
        private void MainWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var positionUiGrid = e.GetPosition(UiGrid);
            if ((!(positionUiGrid.X >= 0) || !(positionUiGrid.X < UiGrid.ActualWidth) || !(positionUiGrid.Y >= 0) ||
                 !(positionUiGrid.Y < UiGrid.ActualHeight))) return;
            if (UiBtnMaximized.Visibility == Visibility.Collapsed)
            {
                UI_btn_normal_Click(null, null);
            }
            else
            {
                UI_btn_maximized_Click(null, null);
            }
        }

        /// <summary>
        /// MainWindow窗口尺寸改变
        /// </summary>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //最大化
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                UI_btn_maximized_Click(null, null);
            }
        }
        #endregion
        
        #region "右上角按钮"
        #region "设置菜单"
        /// <summary>
        /// 设置
        /// </summary>
        private void UI_btn_setting_Click(object sender, RoutedEventArgs e)
        {
            UiPopSetting.IsOpen = true;
        }
        
        /// <summary>
        /// 窗口置顶
        /// </summary>
        private void Se_button_Topmost_Click(object sender, RoutedEventArgs e)
        {
            if (Topmost == false)
            {
                Topmost = true;
                SeImageTopmost.Source = new BitmapImage(new Uri("pack://application:,,,/饥荒开服工具ByTpxxn;component/Resources/Setting_Top_T.png", UriKind.Absolute));
                SeTextblockTopmost.Text = "永远置顶";
                IniFileIo.IniFileWrite("Configure", "Others", "Topmost", "1");
            }
            else
            {
                Topmost = false;
                SeImageTopmost.Source = new BitmapImage(new Uri("pack://application:,,,/饥荒开服工具ByTpxxn;component/Resources/Setting_Top_F.png", UriKind.Absolute));
                SeTextblockTopmost.Text = "永不置顶";
                IniFileIo.IniFileWrite("Configure", "Others", "Topmost", "0");
            }
        }
        #endregion

        #region "皮肤菜单"
        /// <summary>
        /// 皮肤菜单
        /// </summary>
        private void UI_btn_bg_Click(object sender, RoutedEventArgs e)
        {
            UiPopBg.IsOpen = true;
        }

        /// <summary>
        /// 获取字体函数
        /// </summary>
        /// <returns>字体列表</returns>
        private static IEnumerable<string> ReadFont()
        {
            var installedFontCollectionFont = new InstalledFontCollection();
            var fontFamilys = installedFontCollectionFont.Families;
            return fontFamilys.Length < 1 ? null : fontFamilys.Select(item => item.Name).ToList();
        }

        /// <summary>
        /// 修改字体
        /// </summary>
        private void Se_ComboBox_Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!LoadFont) return;
            var textList = (from TextBlock textBlock in SeComboBoxFont.Items select textBlock.Text).ToList();
            mainWindow.FontFamily = new FontFamily(textList[SeComboBoxFont.SelectedIndex]);
            ((TextBlock)((VisualBrush)FindResource("HelpBrush")).Visual).FontFamily = mainWindow.FontFamily;
            Global.FontFamily = mainWindow.FontFamily;
            IniFileIo.IniFileWrite("Configure", "Font", "FontFamily", textList[SeComboBoxFont.SelectedIndex]);
        }

        /// <summary>
        /// 修改字体加粗
        /// </summary>
        private void Se_CheckBox_FontWeight_Click(object sender, RoutedEventArgs e)
        {
            if (!LoadFont) return;
            mainWindow.FontWeight = SeCheckBoxFontWeight.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            ((TextBlock)((VisualBrush)FindResource("HelpBrush")).Visual).FontWeight = mainWindow.FontWeight;
            Global.FontWeight = mainWindow.FontWeight;
            IniFileIo.IniFileWrite("Configure", "Font", "FontWeight", SeCheckBoxFontWeight.IsChecked.ToString());
        }

        /// <summary>
        /// 修改淡紫色透明光标
        /// </summary>
        private void SeCheckBoxLavenderCursor_Click(object sender, RoutedEventArgs e)
        {
            // TODO 提示框
            //var copySplashWindow = new CopySplashScreen("重启生效");
            //copySplashWindow.InitializeComponent();
            //copySplashWindow.ContentTextBlock.FontSize = 20;
            //copySplashWindow.Show();
            IniFileIo.IniFileWrite("Configure", "Others", "LavenderCursor", SeCheckBoxLavenderCursor.IsChecked.ToString());
        }

        /// <summary>
        /// 设置背景
        /// </summary>
        private void Se_button_Background_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = "选择背景图片",
                FileName = "", //默认文件名
                DefaultExt = ".png", // 默认文件扩展名
                Filter = "图像文件 (*.bmp;*.gif;*.jpg;*.jpeg;*.png)|*.bmp;*.gif;*.jpg;*.jpeg;*.png" //文件扩展名过滤器
            };
            // ReSharper disable once UnusedVariable
            var result = openFileDialog.ShowDialog(); //显示打开文件对话框
            UiBackGroundBorder.Visibility = Visibility.Visible;
            try
            {
                var pictruePath = Environment.CurrentDirectory + @"\Background\"; //设置文件夹位置
                if (Directory.Exists(pictruePath) == false) //若文件夹不存在
                {
                    Directory.CreateDirectory(pictruePath);
                }
                var filename = Path.GetFileName(openFileDialog.FileName); //设置文件名
                try
                {
                    File.Copy(openFileDialog.FileName, pictruePath + filename, true);
                }
                catch (Exception)
                {
                    // ignored
                }
                var brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(pictruePath + filename))
                };
                UiBackGroundBorder.Background = brush;
                SeBgAlphaText.Foreground = Brushes.Black;
                SeBgAlpha.IsEnabled = true;
                IniFileIo.IniFileWrite("Configure", "Skin", "Background", pictruePath + filename);
            }
            catch (Exception)
            {
                MessageBox.Show("没有选择正确的图片ヽ(ﾟДﾟ)ﾉ");
            }
        }

        /// <summary>
        /// 清除背景
        /// </summary>
        private void Se_button_Background_Clear_Click(object sender, RoutedEventArgs e)
        {
            UiBackGroundBorder.Visibility = Visibility.Collapsed;
            UiBackGroundBorder.Background = null;
            SeBgAlphaText.Foreground = Brushes.Silver;
            SeBgAlpha.IsEnabled = false;
            IniFileIo.IniFileWrite("Configure", "Skin", "Background", "");
        }

        /// <summary>
        /// 设置背景拉伸方式
        /// </summary>
        private void Se_ComboBox_Background_Stretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var bg = IniFileIo.IniFileReadString("Configure", "Skin", "Background");
            if (!MwInit) return;
            if (bg == "")
            {
                SeBgAlphaText.Foreground = Brushes.Silver;
                SeBgAlpha.IsEnabled = false;
            }
            else
            {
                SeBgAlphaText.Foreground = Brushes.Black;
                try
                {
                    var brush = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(bg)),
                        Stretch = (Stretch)SeComboBoxBackgroundStretch.SelectedIndex
                    };
                    UiBackGroundBorder.Background = brush;
                    IniFileIo.IniFileWrite("Configure", "Skin", "BackgroundStretch", (SeComboBoxBackgroundStretch.SelectedIndex + 1).ToString());
                }
                catch
                {
                    UiBackGroundBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 设置背景透明度
        /// </summary>
        private void Se_BG_Alpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UiBackGroundBorder.Opacity = SeBgAlpha.Value / 100;
            SeBgAlphaText.Text = "背景不透明度：" + (int)SeBgAlpha.Value + "%";
            IniFileIo.IniFileWrite("Configure", "Skin", "BackgroundAlpha", (SeBgAlpha.Value + 1).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 设置面板透明度
        /// </summary>
        private void Se_Panel_Alpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO 面板透明度
            //RightFrame.Background.Opacity = SePanelAlpha.Value / 100;
            //SePanelAlphaText.Text = "面板不透明度：" + (int)SePanelAlpha.Value + "%";
            //RegeditRw.RegWrite("BGPanelAlpha", SePanelAlpha.Value + 1);
        }

        /// <summary>
        /// 设置窗口透明度
        /// </summary>
        private void Se_Window_Alpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = SeWindowAlpha.Value / 100;
            SeWindowAlphaText.Text = "窗口不透明度：" + (int)SeWindowAlpha.Value + "%";
            IniFileIo.IniFileWrite("Configure", "Skin", "WindowAlpha", (SeWindowAlpha.Value + 1).ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        #region "最小化/最大化/关闭按钮"
        public Rect RectNormal;//窗口位置
        /// <summary>
        /// 最小化按钮
        /// </summary>
        private void UI_btn_minimized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 最大化按钮
        /// </summary>
        private void UI_btn_maximized_Click(object sender, RoutedEventArgs e)
        {
            UiBtnMaximized.Visibility = Visibility.Collapsed;
            UiBtnNormal.Visibility = Visibility.Visible;
            RectNormal = new Rect(Left, Top, Width, Height);//保存下当前位置与大小
            Left = 0;
            Top = 0;
            var systemWorkAreaRect = SystemParameters.WorkArea;
            Width = systemWorkAreaRect.Width;
            Height = systemWorkAreaRect.Height;
            //WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 还原按钮
        /// </summary>
        private void UI_btn_normal_Click(object sender, RoutedEventArgs e)
        {
            UiBtnMaximized.Visibility = Visibility.Visible;
            UiBtnNormal.Visibility = Visibility.Collapsed;
            Left = RectNormal.Left;
            Top = RectNormal.Top;
            Width = RectNormal.Width;
            Height = RectNormal.Height;
            //WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void UI_btn_close_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //if (Settings.HideToNotifyIconPrompt == false)
            //{
            //    var notifyIconMessageBox = new NotifyIconMessageBox();
            //    notifyIconMessageBox.ShowDialog();
            //}
            //else
            //{
            //    if (Settings.HideToNotifyIcon)
            //    {
            //        NotifyIcon.ShowBalloonTip(1000);
            //        MwVisibility = false;
            //    }
            //    else
            //    {
            //        DisposeNotifyIcon();
            //        Application.Current.Shutdown();
            //    }
            //}
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
        }

        #region 自定义Alt+F4 和 屏蔽Alt+Space
        private bool _altDown;
        private bool _ctrlDown;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = true;
            }
            else if (e.SystemKey == Key.LeftCtrl || e.SystemKey == Key.RightCtrl)
            {
                _ctrlDown = true;
            }
            // 调用自定义Alt+F4事件
            else if (e.SystemKey == Key.F4 && _altDown)
            {
                e.Handled = true;
                UI_btn_close_Click(null, null);
            }
            // Alt+Space直接屏蔽
            else if (e.SystemKey == Key.Space && _altDown)
            {
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = false;
            }
            if (e.SystemKey == Key.LeftCtrl || e.SystemKey == Key.RightCtrl)
            {
                _ctrlDown = false;
            }
        }
        #endregion

        private void DisposeNotifyIcon()
        {
            // TODO 
            //NotifyIcon.Visible = false;
            //NotifyIcon.Dispose();
        }

        #endregion
        #endregion

    }
}
