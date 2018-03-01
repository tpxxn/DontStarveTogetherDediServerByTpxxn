using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;
using Color = System.Windows.Media.Color;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        #region Mod配置

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
                        Width = 250,
                        Height = 100,
                        ContentMod = _mods.ModList[i],
                        ModSelectCheckBox = { Tag = i },
                    };
                    dediModBox.ModSelectCheckBox.IsChecked = _mods.ModList[i].Enabled;
                    dediModBox.ModSelectCheckBox.Checked += CheckBox_Checked;
                    dediModBox.ModSelectCheckBox.Unchecked += CheckBox_Unchecked;
                    dediModBox.PreviewMouseLeftButtonDown += DediModBox_MouseLeftButtonDown;
                    ModListStackPanel.Children.Add(dediModBox);
                }
            }
            // 自动显示第一个mod的详情
            if (ModListStackPanel.Children.Count != 0)
            {
                DediModBox_MouseLeftButtonDown(ModListStackPanel.Children[0], null);
            }
        }

        /// <summary>
        /// 设置 "Mod" "MouseLeftButtonDown"
        /// </summary>
        private void DediModBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // [Mod信息]
            var n = (int)((DediModBox)sender).ModSelectCheckBox.Tag;
            ModInfoImage.Source = _mods.ModList[n].Picture != null ? PictureHelper.ChangeBitmapToImageSource(_mods.ModList[n].Picture) : null;
            ModInfoNameTextBlock.Text = _mods.ModList[n].Name;
            ModInfoAuthorTextBlock.Text = "作者：" + _mods.ModList[n].Author;
            ModInfoVersionTextBlock.Text = "版本：" + _mods.ModList[n].Version;
            ModInfoFolderTextBlock.Text = "文件夹：" + _mods.ModList[n].ModDirName;
            ModInfoTypeTextBlock.Text = _mods.ModList[n].ModType == ModType.Server ? "服务器" : "所有人";
            // [Mod描述]
            ModDescriptionStackPanel.Text = _mods.ModList[n].Description;
            // [Mod设置]
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
                    // StackPanel
                    var modOptionStackPanel = new StackPanel
                    {
                        Height = 40,
                        Width = 330,
                        Orientation = Orientation.Horizontal
                    };
                    // TextBlock
                    var modOptionTitleTextBlock = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 150,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        FontWeight = FontWeights.Bold,
                        Text = string.IsNullOrEmpty(item.Value.Label) ? item.Value.Name : item.Value.Label
                    };
                    // DediSelectBox
                    var modOptionDediSelectBox = new DediSelectBox
                    {
                        Height = modOptionStackPanel.Height,
                        Width = 150,
                        TextFontSize = 14,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        Tag = n + "$" + item.Key,
                        TextList = item.Value.Options.Select(option => option.Description).ToList()
                    };
                    // 把当前选择mod的第n个,放到tag里
                    modOptionDediSelectBox.TextIndex = CurrentDescriptionToTextIndex(item.Value.CurrentDescription, modOptionDediSelectBox.TextList);
                    modOptionDediSelectBox.SelectionChangedWithSender += DediSelectBox_SelectionChanged;
                    // 添加
                    modOptionStackPanel.Children.Add(modOptionTitleTextBlock);
                    modOptionStackPanel.Children.Add(modOptionDediSelectBox);
                    ModSettingStackPanel.Children.Add(modOptionStackPanel);
                }
                UpdateLayout();
            }
        }

        private int CurrentDescriptionToTextIndex(string currentDescription, List<string> dediSelectBoxTextList)
        {
            for (var i = 0; i < dediSelectBoxTextList.Count; i++)
            {
                if (currentDescription == dediSelectBoxTextList[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 设置 "Mod" "SelectionChanged"
        /// </summary>
        private void DediSelectBox_SelectionChanged(object sender)
        {
            Debug.WriteLine(((DediSelectBox)sender).Tag);
            var str = ((DediSelectBox)sender).Tag.ToString().Split('$');
            if (str.Length != 0)
            {
                var n = int.Parse(str[0]);
                var name = str[1];
                // 好复杂
                _mods.ModList[n].ConfigurationOptions[name].Current =
                    _mods.ModList[n].ConfigurationOptions[name].Options[((DediSelectBox)sender).TextIndex].Data;

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

        #endregion

        #region Mod管理

        /// <summary>
        /// 验证mod id
        /// </summary>
        /// <param name="dialogWindowWithButton"></param>
        private static void ValidateModId(DialogWindowWithButton dialogWindowWithButton)
        {
            var modIdString = dialogWindowWithButton.InputTextBox.Text;
            if (modIdString.Length >= 9 && modIdString.Length <= 10)
            {
                double.TryParse(modIdString, out var modId);
                dialogWindowWithButton.Result = modId;
            }
            else
                dialogWindowWithButton.Result = (double)0;
        }

        /// <summary>
        /// 从创意工坊添加mod
        /// </summary>
        private async void AddModButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempPath = Environment.CurrentDirectory + @"\Temp\";
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);

                var dialogWindowWithButton = new DialogWindowWithButton("请输入mod ID", DialogWindowWithButton.DialogButtons.OKCancel);
                dialogWindowWithButton.InitializeComponent();
                dialogWindowWithButton.OKbuttonEvent += ValidateModId;
                dialogWindowWithButton.ShowDialog();
                var modIdFromResult = (double)dialogWindowWithButton.Result;
                if (modIdFromResult == 0)
                {
                    Debug.WriteLine("错误的mod ID");
                    return;
                }
                var modDirName = CommonPath.ServerModsDirPath + "\\workshop-" + modIdFromResult;

                var dialogWindow = new DialogWindow(modIdFromResult + "下载中") { Owner = Application.Current.MainWindow };
                dialogWindow.InitializeComponent();
                MainGrid.IsEnabled = false;
                dialogWindow.Width = 450;
                dialogWindow.Height = 350;
                dialogWindow.DialogWindowCanvas.Width = 430;
                dialogWindow.DialogWindowCanvas.Height = 340;
                dialogWindow.Show();
                await Task.Run(() =>
                {
                    try
                    {
                        // 下载
                        var modDownloadObject = ModDownloadHelper.DownloadModFromId(modIdFromResult.ToString());
                        ModDownloadHelper.DownloadModFile(modDownloadObject);
                        // 解压
                        ZipFile.ExtractToDirectory(@".\Temp\ModUpdate\workshop-" + modIdFromResult + ".zip",
                            modDirName + ".tmp");
                        if (Directory.Exists(modDirName))
                            Directory.Delete(modDirName, true);
                        new FileInfo(modDirName + ".tmp").MoveTo(modDirName);
                    }
                    catch
                    {
                        // ignored
                    }
                });
                MainGrid.IsEnabled = true;
                dialogWindow.Close();
                RefreshModButton_OnClick(null, null);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        /// <summary>
        /// 更新全部mod
        /// </summary>
        private async void UpdateAllModButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempPath = Environment.CurrentDirectory + @"\Temp\";
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);
                foreach (var mod in _mods.ModList)
                {
                    var dialogWindow = new DialogWindow(mod.Name + "下载中") { Owner = Application.Current.MainWindow };
                    dialogWindow.InitializeComponent();
                    MainGrid.IsEnabled = false;
                    dialogWindow.Width = 450;
                    dialogWindow.Height = 350;
                    dialogWindow.DialogWindowCanvas.Width = 430;
                    dialogWindow.DialogWindowCanvas.Height = 340;
                    dialogWindow.Show();
                    await Task.Run(() =>
                    {
                        try
                        {
                            // 下载
                            var modId = mod.ModDirName.Replace("workshop-", "");
                            var modDownloadObject = ModDownloadHelper.DownloadModFromId(modId);
                            ModDownloadHelper.DownloadModFile(modDownloadObject);
                            // 解压
                            ZipFile.ExtractToDirectory(@".\Temp\ModUpdate\workshop-" + modId + ".zip",
                                CommonPath.ServerModsDirPath + "\\" + mod.ModDirName + ".tmp");
                            if (Directory.Exists(CommonPath.ServerModsDirPath + "\\" + mod.ModDirName))
                                Directory.Delete(CommonPath.ServerModsDirPath + "\\" + mod.ModDirName, true);
                            new FileInfo(CommonPath.ServerModsDirPath + "\\" + mod.ModDirName + ".tmp").MoveTo(CommonPath.ServerModsDirPath + "\\" + mod.ModDirName);
                        }
                        catch
                        {
                            // ignored
                        }
                    });
                    MainGrid.IsEnabled = true;
                    dialogWindow.Close();
                }
                RefreshModButton_OnClick(null, null);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        /// <summary>
        /// 刷新mod列表[重新读取mods文件夹和modoverrides.lua]
        /// </summary>
        private async void RefreshModButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new DialogWindow("mod加载中") { Owner = Application.Current.MainWindow };
            dialogWindow.InitializeComponent();
            MainGrid.IsEnabled = false;
            dialogWindow.Show();
            _mods = null;
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath))
            {
                _mods = await Task.Run(() => new Mods());
            }
            SetModSet();
            MainGrid.IsEnabled = true;
            dialogWindow.Close();
        }

        #endregion
    }
}
