using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 饥荒开服工具ByTpxxn.MyUserControl
{
    /// <summary>
    /// DediEditWorldSelectBox.xaml 的交互逻辑
    /// </summary>
    public partial class DediEditWorldSelectBox : UserControl
    {

        #region 属性：ImageSource

        public ImageSource ImageSource
        {
            set => SetValue(ImageSourceProperty, value);
            get => (ImageSource)GetValue(ImageSourceProperty);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(DediEditWorldSelectBox), new PropertyMetadata(null, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            dediSelectBox.Picture.Source = (ImageSource)e.NewValue;
        }

        #endregion

        #region 属性：ImageToolTip
        public string ImageToolTip
        {
            set => SetValue(ImageToolTipProperty, value);
            get => (string)GetValue(ImageToolTipProperty);
        }

        public static readonly DependencyProperty ImageToolTipProperty =
            //DependencyProperty.Register("ImageToolTip", typeof(string), typeof(DediEditWorldSelectBox));
        DependencyProperty.Register("ImageToolTip", typeof(string), typeof(DediEditWorldSelectBox), new PropertyMetadata(string.Empty, OnImageToolTipChanged));

        private static void OnImageToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            //dediSelectBox.Picture.ToolTip = (string)e.NewValue;
            dediSelectBox.Picture.ToolTip = new ToolTip
            {
                BorderBrush = null,
                Foreground = null,
                Placement = PlacementMode.Top,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/饥荒开服工具ByTpxxn;component/Resources/DedicatedServer/D_mp_btn_tip.png", UriKind.RelativeOrAbsolute))
                },
                Content = new TextBlock
                {
                    Text = (string)e.NewValue,
                    Foreground = Brushes.White,
                    FontSize = 15,
                    Margin = new Thickness(10, 2, 10, 2)
                }
            };
            dediSelectBox.TitleText = (string)e.NewValue;
        }
        #endregion

        #region 属性：Text
        public string Text { get; set; }
        #endregion

        #region 属性：ControlWidth
        public double ControlWidth
        {
            get => (double)GetValue(ControlWidthProperty);
            set => SetValue(ControlWidthProperty, value);
        }

        public static readonly DependencyProperty ControlWidthProperty =
            DependencyProperty.Register("ControlWidth", typeof(double), typeof(DediEditWorldSelectBox), new PropertyMetadata((double)160, OnControlWidthChange));

        private static void OnControlWidthChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            if ((double)e.NewValue <= 51.2)
            {
                dediSelectBox.Border.Width = 160;
                dediSelectBox.Grid.Width = 150;
            }
            else
            {
                dediSelectBox.Border.Width = (double)e.NewValue;
                dediSelectBox.Grid.Width = (double)e.NewValue - 10;
            }
        }
        #endregion

        #region 属性：TextIndex
        public int TextIndex
        {
            get => (int)GetValue(TextIndexProperty);
            set => SetValue(TextIndexProperty, value);
        }

        public static readonly DependencyProperty TextIndexProperty =
            DependencyProperty.Register("TextIndex", typeof(int), typeof(DediEditWorldSelectBox), new PropertyMetadata(0, OnTextIndexChange));

        private static void OnTextIndexChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            dediSelectBox.ContentTextBlock.Text = dediSelectBox.TextList[(int)e.NewValue];
            dediSelectBox.Text = dediSelectBox.TextList[(int)e.NewValue];
            if ((int)e.NewValue == dediSelectBox.TextList.Count - 1)
            {
                dediSelectBox.SwitchLeftButton.IsEnabled = true;
                dediSelectBox.SwitchRightButton.IsEnabled = false;
            }
        }
        #endregion

        #region 属性：TextList
        public List<string> TextList
        {
            get => (List<string>)GetValue(TextListProperty);
            set => SetValue(TextListProperty, value);
        }

        public static readonly DependencyProperty TextListProperty =
            DependencyProperty.Register("TextList", typeof(List<string>), typeof(DediEditWorldSelectBox), new PropertyMetadata(null, OnTextListChange));

        private static void OnTextListChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            dediSelectBox.ContentTextBlock.Visibility = Visibility.Visible;
            dediSelectBox.ContentTextBlock.Text = ((List<string>)e.NewValue)[dediSelectBox.TextIndex];
        }
        #endregion

        #region 属性：TitleText
        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(DediEditWorldSelectBox), new PropertyMetadata(string.Empty, OnTitleTextChange));

        private static void OnTitleTextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediEditWorldSelectBox)d;
            dediSelectBox.TitleTextBlock.Text = (string)e.NewValue;
        }
        #endregion

        #region 左右切换按钮
        private void SwitchLeftButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchRightButton.IsEnabled = true;
            if (TextIndex != 0)
            {
                TextIndex -= 1;
                if (TextIndex == 0)
                {
                    SwitchLeftButton.IsEnabled = false;
                }
            }
            ContentTextBlock.Text = TextList[TextIndex];
            SelectionChanged?.Invoke();
        }

        private void SwitchRightButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchLeftButton.IsEnabled = true;
            if (TextIndex != TextList.Count - 1)
            {
                TextIndex += 1;
                if (TextIndex == TextList.Count - 1)
                {
                    SwitchRightButton.IsEnabled = false;
                }
            }
            ContentTextBlock.Text = TextList[TextIndex];
            SelectionChanged?.Invoke();
        }

        /// <summary>
        /// SelectChanged事件委托
        /// </summary>
        public delegate void SelectionChangedEventHandler();

        /// <summary>
        /// SelectChanged事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion
        public DediEditWorldSelectBox()
        {
            InitializeComponent();
        }
    }
}
