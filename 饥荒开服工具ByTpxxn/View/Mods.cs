using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;
using Color = System.Windows.Media.Color;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        private static bool serverModsDirWatcherIsEnable = true;
        #region Mod配置

        /// <summary>
        /// 设置 "Mod集"
        /// </summary>
        private void SetModSet()
        {   // 设置
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath))
            {
                // 清空,Enabled变成默认值
                foreach (var mod in _mods.ModList)
                {
                    mod.Enabled = false;
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
            ModDescriptionStackPanel.Text = "";
            ModSettingStackPanel.Children.Clear();
            if (_mods != null)
            {
                for (var i = 0; i < _mods.ModList.Count; i++)
                {
                    var dediModBox = new DediModBox
                    {
                        Width = 250,
                        Height = 100,
                        ContentMod = _mods.ModList[i],
                        ModSelectCheckBox = { Tag = i },// 表示CheckBox的Tag属性为第i个mod(i从0开始)
                    };
                    // mod勾选相关事件
                    dediModBox.ModSelectCheckBox.IsChecked = _mods.ModList[i].Enabled;
                    dediModBox.ModSelectCheckBox.Checked += CheckBox_Checked;
                    dediModBox.ModSelectCheckBox.Unchecked += CheckBox_Unchecked;
                    // 点击DediModBox查看mod信息
                    dediModBox.PreviewMouseLeftButtonDown += DediModBox_Select;
                    // 把dediModBox添加到ModListStackPanel
                    ModListStackPanel.Children.Add(dediModBox);
                }
            }
            // 自动显示第一个mod的详情
            if (ModListStackPanel.Children.Count != 0)
            {
                DediModBox_Select(ModListStackPanel.Children[0], null);
            }
        }

        /// <summary>
        /// 点击DediModBox查看mod信息
        /// </summary>
        private void DediModBox_Select(object sender, MouseButtonEventArgs e)
        {
            // [Mod信息]
            var modIndex = (int)((DediModBox)sender).ModSelectCheckBox.Tag;
            ModInfoImage.Source = _mods.ModList[modIndex].Picture != null ? PictureHelper.ChangeBitmapToImageSource(_mods.ModList[modIndex].Picture) : null;
            ModInfoNameTextBlock.Text = _mods.ModList[modIndex].Name;
            ModInfoAuthorTextBlock.Text = "作者：" + _mods.ModList[modIndex].Author;
            ModInfoVersionTextBlock.Text = "版本：" + _mods.ModList[modIndex].Version;
            ModInfoFolderTextBlock.Text = "文件夹：" + _mods.ModList[modIndex].ModDirName;
            ModInfoTypeTextBlock.Text = _mods.ModList[modIndex].ModType == ModType.Server ? "服务器" : "所有人";
            // [Mod描述]
            ModDescriptionStackPanel.Text = _mods.ModList[modIndex].Description;
            // [Mod设置]
            if (_mods.ModList[modIndex].ConfigurationOptions.Count == 0)
            {
                // 没有细节配置项
                Debug.WriteLine(_mods.ModList[modIndex].Name + "没有配置选项");
                ModSettingStackPanel.Children.Clear();
            }
            else
            {
                // 有,显示细节配置项
                Debug.WriteLine(_mods.ModList[modIndex].Name + "的配置选项");
                ModSettingStackPanel.Children.Clear();
                foreach (var modSetting in _mods.ModList[modIndex].ConfigurationOptions)
                {
                    // 包含mod设置标题和选择框的StackPanel
                    var singleModSettingStackPanel = new StackPanel
                    {
                        Height = 40,
                        Width = 330,
                        Orientation = Orientation.Horizontal
                    };
                    // mod设置的标题
                    var modSettingTitleTextBlock = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 150,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        FontWeight = FontWeights.Bold,
                        Text = string.IsNullOrEmpty(modSetting.Value.Label) ? modSetting.Value.Name : modSetting.Value.Label
                    };
                    // mod设置的选择框
                    var modSettingDediSelectBox = new DediSelectBox
                    {
                        Height = singleModSettingStackPanel.Height,
                        Width = 150,
                        TextFontSize = 14,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        Tag = modIndex + "$" + modSetting.Key,// mod序号+$+modSetting的键
                        TextList = modSetting.Value.Options.Select(option => option.Description).ToList()
                    };
                    // 把当前选择mod的第n个,放到tag里
                    modSettingDediSelectBox.TextIndex = CurrentDescriptionToTextIndex(modSetting.Value.CurrentDescription, modSettingDediSelectBox.TextList);
                    modSettingDediSelectBox.SelectionChangedWithSender += DediSelectBox_SelectionChanged;
                    // 添加
                    singleModSettingStackPanel.Children.Add(modSettingTitleTextBlock);
                    singleModSettingStackPanel.Children.Add(modSettingDediSelectBox);
                    ModSettingStackPanel.Children.Add(singleModSettingStackPanel);
                }
                UpdateLayout();
            }
        }

        /// <summary>
        /// 把当前选择项放在DediSelectBox的对应TextIndex
        /// </summary>
        /// <param name="currentDescription">当前的描述</param>
        /// <param name="dediSelectBoxTextList">选项列表</param>
        /// <returns>选项索引</returns>
        private static int CurrentDescriptionToTextIndex(string currentDescription, List<string> dediSelectBoxTextList)
        {
            for (var i = 0; i < dediSelectBoxTextList.Count; i++)
            {
                if (currentDescription == dediSelectBoxTextList[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// mod选项改变事件
        /// </summary>
        private void DediSelectBox_SelectionChanged(object sender)
        {
            Debug.WriteLine(((DediSelectBox)sender).Tag);
            var dediSelectBoxTag = ((DediSelectBox)sender).Tag.ToString().Split('$');
            if (dediSelectBoxTag.Length != 0)
            {
                var modIndex = int.Parse(dediSelectBoxTag[0]);
                var modSettingKey = dediSelectBoxTag[1];
                // 设置当前选项
                _mods.ModList[modIndex].ConfigurationOptions[modSettingKey].Current = _mods.ModList[modIndex].ConfigurationOptions[modSettingKey].Options[((DediSelectBox)sender).TextIndex].Data;
            }
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Unchecked"
        /// </summary>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((DediModBox)sender).Tag].Enabled = false;
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Checked"
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((DediModBox)sender).Tag].Enabled = true;
        }

        #endregion

        #region Mod管理

        /// <summary>
        /// 从创意工坊添加mod
        /// </summary>
        private async void AddModButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                serverModsDirWatcherIsEnable = false;
                var tempPath = Environment.CurrentDirectory + @"\Temp\";
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);

                var dialogWindowWithButton = new DialogWindowWithButton("请输入mod ID", DialogWindowWithButton.DialogButtons.OKCancel);
                dialogWindowWithButton.InitializeComponent();
                dialogWindowWithButton.OKbuttonEvent += Sender =>
                {
                    var modIdString = Sender.InputTextBox.Text;
                    if (modIdString.Length >= 9 && modIdString.Length <= 10)
                    {
                        double.TryParse(modIdString, out var modId);
                        Sender.Result = modId;
                    }
                    else
                        Sender.Result = (double)0;
                };
                dialogWindowWithButton.ShowDialog();
                var modIdFromResult = (double)dialogWindowWithButton.Result;
                if (modIdFromResult == 0)
                {
                    Debug.WriteLine("无效的mod ID");
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
                        var modDownloadObject = ModDownloadHelper.DownloadModFromId(modIdFromResult.ToString(CultureInfo.InvariantCulture));
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
                serverModsDirWatcherIsEnable = true;
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
                serverModsDirWatcherIsEnable = false;
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
                serverModsDirWatcherIsEnable = true;
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
        
        /// <summary>
        /// 服务器mod文件夹监控
        /// </summary>
        private void ServerModsDirWatcherStart()
        {
            Global.ServerModsDirWatcher = new FileSystemWatcher
            {
                Path = CommonPath.ServerModsDirPath,
                NotifyFilter = 
                NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName
                | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite 
                | NotifyFilters.Security | NotifyFilters.Size,// 全监控
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            Global.ServerModsDirWatcher.Changed += OnServerModsDirChanged;
            Global.ServerModsDirWatcher.Created += OnServerModsDirChanged;
            Global.ServerModsDirWatcher.Deleted += OnServerModsDirChanged;
            Global.ServerModsDirWatcher.Renamed += OnServerModsDirChanged;
        }

        /// <summary>
        /// 服务器mod文件夹发生改变
        /// </summary>
        private void OnServerModsDirChanged(object source, FileSystemEventArgs e)
        {
            if (serverModsDirWatcherIsEnable)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    serverModsDirWatcherIsEnable = false;
                    var dialogWindowWithButton = new DialogWindowWithButton("服务器mod文件夹发生变化，是否重新加载mod列表", DialogWindowWithButton.DialogButtons.OKCancel, true);
                    dialogWindowWithButton.InitializeComponent();
                    dialogWindowWithButton.OKbuttonEvent += sender => sender.Result = true;
                    dialogWindowWithButton.ShowDialog();
                    if ((bool?)dialogWindowWithButton.Result == true)
                    {
                        RefreshModButton_OnClick(null, null);
                    }
                    var timer = new System.Windows.Forms.Timer
                    {
                        Interval = 1000,
                        Enabled = true
                    };
                    timer.Tick += (sender, eventArgs) =>
                    {
                        serverModsDirWatcherIsEnable = true;
                        ((System.Windows.Forms.Timer)sender).Stop();
                    };
                    timer.Start();
                });
            }
        }

        #endregion
    }
}
