﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 饥荒开服工具ByTpxxn.MyUserControl
{
    /// <summary>
    /// DediSelectBox.xaml 的交互逻辑
    /// </summary>
    public partial class DediSelectBox : UserControl
    {
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
            DependencyProperty.Register("ControlWidth", typeof(double), typeof(DediSelectBox), new PropertyMetadata((double)160, OnControlWidthChange));

        private static void OnControlWidthChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediSelectBox)d;
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

        #region 属性：TextFontSize

        public double TextFontSize
        {
            get => (double)GetValue(TextFontSizeProperty);
            set => SetValue(TextFontSizeProperty, value);
        }

        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(DediSelectBox), new PropertyMetadata((double)0, OnTextFontSizeChange));

        private static void OnTextFontSizeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediSelectBox)d;
            dediSelectBox.TextBlock.FontSize = (double)e.NewValue;
        }
        #endregion

        #region 属性：TextIndex

        public int TextIndex
        {
            get => (int)GetValue(TextIndexProperty);
            set => SetValue(TextIndexProperty, value);
        }

        public static readonly DependencyProperty TextIndexProperty =
            DependencyProperty.Register("TextIndex", typeof(int), typeof(DediSelectBox), new PropertyMetadata(0, OnTextIndexChange));

        private static void OnTextIndexChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediSelectBox)d;
            dediSelectBox.TextBlock.Text = dediSelectBox.TextList[(int)e.NewValue];
            dediSelectBox.Text = dediSelectBox.TextList[(int)e.NewValue];
            if ((int)e.NewValue == dediSelectBox.TextList.Count - 1)
            {
                dediSelectBox.SwitchLeftButton.IsEnabled = true;
                dediSelectBox.SwitchRightButton.IsEnabled = false;
            }
            else if ((int)e.NewValue == 0)
            {
                dediSelectBox.SwitchLeftButton.IsEnabled = false;
                dediSelectBox.SwitchRightButton.IsEnabled = true;
            }
            else
            {
                dediSelectBox.SwitchLeftButton.IsEnabled = true;
                dediSelectBox.SwitchRightButton.IsEnabled = true;
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
            DependencyProperty.Register("TextList", typeof(List<string>), typeof(DediSelectBox), new PropertyMetadata(null, OnTextListChange));

        private static void OnTextListChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediSelectBox = (DediSelectBox)d;
            dediSelectBox.TextBlock.Visibility = Visibility.Visible;
            dediSelectBox.TextBlock.Text = ((List<string>)e.NewValue)[dediSelectBox.TextIndex];
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
            TextBlock.Text = TextList[TextIndex];
            SelectionChanged?.Invoke();
            SelectionChangedWithSender?.Invoke(this);
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
            TextBlock.Text = TextList[TextIndex];
            SelectionChanged?.Invoke();
            SelectionChangedWithSender?.Invoke(this);
        }

        #endregion

        #region [SelectChanged|SelectionChangedWithSender]事件委托

        /// <summary>
        /// SelectChanged事件委托
        /// </summary>
        public delegate void SelectionChangedEventHandler();


        /// <summary>
        /// SelectChanged事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// SelectionChangedWithSender事件委托
        /// </summary>
        public delegate void SelectionChangedWithSenderEventHandler(object sender);


        /// <summary>
        /// SelectionChangedWithSender事件
        /// </summary>
        public event SelectionChangedWithSenderEventHandler SelectionChangedWithSender;

        #endregion
        public DediSelectBox()
        {
            InitializeComponent();
        }
    }
}
