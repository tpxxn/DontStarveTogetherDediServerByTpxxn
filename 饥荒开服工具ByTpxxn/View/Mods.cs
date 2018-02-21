using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        /// <summary>
        /// 设置 "Mod集"
        /// </summary>
        private void SetModSet()
        {   // 设置
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath))
            {
                // 清空,Enabled变成默认值
                foreach (var item in _mods.ModList)
                {
                    item.Enabled = false;
                }
                // 细节也要变成默认值,之后再重新读取
                foreach (var mod in _mods.ModList)
                {
                    foreach (var configurationOption in mod.ConfigurationOptions)
                    {
                        configurationOption.Value.Current = configurationOption.Value.Default;
                    }
                }
                // 重新读取
                _mods.ReadModsOverrides(_dediFilePath.ModConfigOverworldFilePath);
            }
            // 显示 
            ModListStackPanel.Children.Clear();
            ModSettingStackPanel.Children.Clear();
            ModDescriptionStackPanel.Text = "";
            if (_mods != null)
            {
                for (var i = 0; i < _mods.ModList.Count; i++)
                {
                    var dediModBox = new DediModBox
                    {
                        Width = 200,
                        Height = 70,
                        UCTitle = { Content = _mods.ModList[i].Name },
                        UCCheckBox = { Tag = i },
                        UCConfig =
                        {
                            Source = _mods.ModList[i].ConfigurationOptions.Count != 0
                                ? new BitmapImage(new Uri(
                                    "/饥荒开服工具ByTpxxn;component/Resources/DedicatedServer/D_mp_mod_config.png",
                                    UriKind.Relative))
                                : null
                        }
                    };
                    dediModBox.UCCheckBox.IsChecked = _mods.ModList[i].Enabled;
                    dediModBox.UCCheckBox.Checked += CheckBox_Checked;
                    dediModBox.UCCheckBox.Unchecked += CheckBox_Unchecked;
                    dediModBox.PreviewMouseLeftButtonDown += DediModBox_MouseLeftButtonDown;
                    dediModBox.UCEnableLabel.Content = _mods.ModList[i].ModType;
                    ModListStackPanel.Children.Add(dediModBox);
                }
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>  
        /// 从bitmap转换成ImageSource  
        /// </summary>  
        /// <param name="icon"></param>  
        /// <returns></returns>  
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();  
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 设置 "Mod" "MouseLeftButtonDown"
        /// </summary>
        private void DediModBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 左边显示
            var n = (int)((DediModBox)sender).UCCheckBox.Tag;
            ModInfoImage.Source = _mods.ModList[n].Picture != null ? ChangeBitmapToImageSource(_mods.ModList[n].Picture) : null;
            ModInfoNameTextBlock.Text = _mods.ModList[n].Name;
            ModInfoAuthorTextBlock.Text = "作者：" + _mods.ModList[n].Author;
            ModInfoVersionTextBlock.Text = "版本：" + _mods.ModList[n].Version;
            ModInfoFolderTextBlock.Text = "文件夹：" + _mods.ModList[n].ModDirName;
            ModInfoTypeTextBlock.Text = _mods.ModList[n].ModType == ModType.Server ? "服务端" : "所有人";

            ModDescriptionStackPanel.Text =_mods.ModList[n].Description;
            if (_mods.ModList[n].ConfigurationOptions.Count == 0)
            {
                // 没有细节配置项
                Debug.WriteLine(n);
                ModSettingStackPanel.Children.Clear();
            }
            else
            {
                // 有,显示细节配置项
                Debug.WriteLine(n);
                ModSettingStackPanel.Children.Clear();
                foreach (var item in _mods.ModList[n].ConfigurationOptions)
                {
                    // stackPanel
                    var stackPanel = new StackPanel
                    {
                        Height = 40,
                        Width = 330,
                        Orientation = Orientation.Horizontal
                    };
                    var labelModXiJie = new Label
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 150,
                        FontWeight = FontWeights.Bold,
                        Content = string.IsNullOrEmpty(item.Value.Label) ? item.Value.Name : item.Value.Label
                    };
                    // dediComboBox
                    var dediComboBox = new DediComboBox
                    {
                        Height = stackPanel.Height,
                        Width = 150,
                        FontSize = 12,
                        Tag = n + "$" + item.Key
                    };
                    // 把当前选择mod的第n个,放到tag里
                    foreach (var option in item.Value.Options)
                    {
                        dediComboBox.Items.Add(option.Description);
                    }
                    dediComboBox.SelectedValue = item.Value.CurrentDescription;
                    dediComboBox.SelectionChanged += DediComboBox_SelectionChanged;
                    // 添加
                    stackPanel.Children.Add(labelModXiJie);
                    stackPanel.Children.Add(dediComboBox);
                    ModSettingStackPanel.Children.Add(stackPanel);
                }
            }
        }

        /// <summary>
        /// 设置 "Mod" "SelectionChanged"
        /// </summary>
        private void DediComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(((DediComboBox)sender).Tag);
            var str = ((DediComboBox)sender).Tag.ToString().Split('$');
            if (str.Length != 0)
            {
                var n = int.Parse(str[0]);
                var name = str[1];
                // 好复杂
                _mods.ModList[n].ConfigurationOptions[name].Current =
                    _mods.ModList[n].ConfigurationOptions[name].Options[((DediComboBox)sender).SelectedIndex].Data;

            }
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Unchecked"
        /// </summary>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((CheckBox)sender).Tag].Enabled = false;
            //Debug.WriteLine(((CheckBox)sender).Tag.ToString());
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Checked"
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((CheckBox)sender).Tag].Enabled = true;
            //Debug.WriteLine(((CheckBox)sender).Tag.ToString());
        }
    }
}
