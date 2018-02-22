using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;


namespace 饥荒开服工具ByTpxxn.MyUserControl
{
    /// <summary>
    /// DediModBox.xaml 的交互逻辑
    /// </summary>
    public partial class DediModBox : RadioButton
    {
        #region 属性：ImageSource[图片源]

        public Mod ContentMod
        {
            set => SetValue(ContentModProperty, value);
            get => (Mod)GetValue(ContentModProperty);
        }

        public static readonly DependencyProperty ContentModProperty =
            DependencyProperty.Register("ImageSource", typeof(Mod), typeof(DediModBox), new PropertyMetadata(null, OnContentModChanged));

        private static void OnContentModChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var dediModBox = (DediModBox)d;
            var mod = (Mod)e.NewValue;
            dediModBox.ModImage.Source = mod.Picture != null ? PictureHelper.ChangeBitmapToImageSource(mod.Picture) : null;
            dediModBox.ModNameTextBlock.Text = mod.Name;
            var modType = mod.ModType;
            if (modType == ModType.Server)
            {
                dediModBox.ModTypeTextBlock.Text = "服务器";
                dediModBox.ModTypeTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(53,198,88));
            }
            else if (modType == ModType.AllClient)
            {
                dediModBox.ModTypeTextBlock.Text = "所有人";
                dediModBox.ModTypeTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 88));
            }
            dediModBox.ModConfig.Source = mod.ConfigurationOptions.Count != 0
                ? new BitmapImage(new Uri(
                    "/饥荒开服工具ByTpxxn;component/Resources/DedicatedServer/ModBox/ModBoxConfig.png",
                    UriKind.Relative))
                : null;
        }

        #endregion

        public DediModBox()
        {
            InitializeComponent();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ModSelectCheckBox.IsChecked = !ModSelectCheckBox.IsChecked;
        }
    }
}
