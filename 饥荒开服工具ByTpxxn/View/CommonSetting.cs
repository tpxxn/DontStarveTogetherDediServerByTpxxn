using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        /// <summary>
        /// 检查通用设置
        /// </summary>
        /// <param name="onCommonSettingPanel">是否已经在通用设置面板</param>
        private void CheckCommonSetting(bool onCommonSettingPanel = false)
        {
            // 读取通用设置
            SetPath();
            // 判断通用设置是否设置完毕
            if (string.IsNullOrEmpty(CommonPath.ClientFilePath) || string.IsNullOrEmpty(CommonPath.ServerFilePath) || string.IsNullOrEmpty(CommonPath.ClusterToken))
            {
                if (onCommonSettingPanel == false)
                {
                    CommonSettingSetOverTextBlock.Text = "请先设定好通用设置！";
                    CommonSettingSetOverTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
                Forbidden(true);
                SaveSlotLeftPanelCenterScrollViewer.IsEnabled = false;
                PanelVisibility("CommonSetting");
            }
            else
            {
                SaveSlotLeftPanelCenterScrollViewer.IsEnabled = true;
                CommonSettingSetOverTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                CommonSettingSetOverTextBlock.Text = "通用设置设定完毕，现在可以在左侧选择存档开启服务器";
            }
        }

        /// <summary>
        /// 设置"路径"
        /// </summary>
        private void SetPath()
        {
            // 读取游戏平台
            CommonPath.SetGamePlatform();
            #region 读取客户端路径、服务器路径和ClusterToken
            // 客户端路径
            GameDirSelectTextBox.Text = "";
            CommonPath.ClientFilePath = CommonPath.ReadClientPath(CommonPath.GamePlatform);
            if (!string.IsNullOrEmpty(CommonPath.ClientFilePath) && File.Exists(CommonPath.ClientFilePath))
                GameDirSelectTextBox.Text = CommonPath.ClientFilePath;
            else
                CommonPath.ClientFilePath = "";
            // 服务器路径
            DediDirSelectTextBox.Text = "";
            CommonPath.ServerFilePath = CommonPath.ReadServerPath(CommonPath.GamePlatform);
            if (!string.IsNullOrEmpty(CommonPath.ServerFilePath) && File.Exists(CommonPath.ServerFilePath))
                DediDirSelectTextBox.Text = CommonPath.ServerFilePath;
            else
                CommonPath.ServerFilePath = "";
            // ClusterToken
            DediSettingClusterTokenTextBox.Text = "";
            CommonPath.ClusterToken = CommonPath.ReadClusterTokenPath(CommonPath.GamePlatform);
            if (!string.IsNullOrEmpty(CommonPath.ClusterToken))
                DediSettingClusterTokenTextBox.Text = CommonPath.ClusterToken;
            else
                CommonPath.ClusterToken = "";
            #endregion
        }

        #region CommonSetting控件事件

        /// <summary>
        /// 游戏平台改变,初始化一切
        /// </summary>
        private void GamePlatformSelectBox_SelectionChanged()
        {
            // 游戏平台
            CommonPath.GamePlatform = GamePlatformSelectBox.TextList[GamePlatformSelectBox.TextIndex];
            if (CommonPath.GamePlatform == "WeGame")
            {
                CtrateRunGame.Visibility = Visibility.Collapsed;
                CtrateWorldButton.Content = "保存世界";
            }
            else
            {
                CtrateRunGame.Visibility = Visibility.Visible;
                CtrateWorldButton.Content = "创建世界";
            }
            // 检查通用设置
            CheckCommonSetting();
            // 初始化
            InitServer();
        }

        /// <summary>
        /// 双击打开"客户端"所在文件夹
        /// </summary>
        private void OpenGameDir(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(CommonPath.ClientFilePath) && File.Exists(CommonPath.ClientFilePath))
            {
                Process.Start(Path.GetDirectoryName(CommonPath.ClientFilePath) ?? throw new InvalidOperationException());
            }
        }

        /// <summary>
        /// 选择游戏exe文件
        /// </summary>
        private void SelectGameDir(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择游戏exe文件",
                FileName = CommonPath.GamePlatform == "WeGame"
                    ? "dontstarve_rail"
                    : "dontstarve_steam", //默认文件名
                DefaultExt = ".exe",// 默认文件扩展名
                Filter = CommonPath.GamePlatform == "WeGame"
                    ? "饥荒游戏exe文件(*.exe)|dontstarve_rail.exe"
                    : "饥荒游戏exe文件(*.exe)|dontstarve_steam.exe",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                if (string.IsNullOrEmpty(fileName) || !fileName.Contains("dontstarve_"))
                {
                    MessageBox.Show("文件选择错误,请选择正确文件");
                    return;
                }
                CommonPath.ClientFilePath = fileName;
                GameDirSelectTextBox.Text = fileName;
                CommonPath.WriteClientPath(fileName, CommonPath.GamePlatform);
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }

        /// <summary>
        /// 双击打开"服务端"所在文件夹
        /// </summary>
        private void OpenDediDir(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(CommonPath.ServerFilePath) && File.Exists(CommonPath.ServerFilePath))
            {
                Process.Start(Path.GetDirectoryName(CommonPath.ServerFilePath) ?? throw new InvalidOperationException());
            }
        }

        /// <summary>
        /// 选择服务器文件
        /// </summary>
        private void SelectDediDir(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择服务器exe文件",
                FileName = "dontstarve_dedicated_server_nullrenderer", //默认文件名
                DefaultExt = ".exe",// 默认文件扩展名
                Filter = "饥荒服务器exe文件(*.exe)|dontstarve_dedicated_server_nullrenderer.exe",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                if (string.IsNullOrEmpty(fileName) || !fileName.Contains("dontstarve_dedicated_server_nullrenderer"))
                {
                    MessageBox.Show("文件选择错误,请选择正确文件");
                    return;
                }
                // 判断是否选择了客户端目录下的的服务器程序
                if (!fileName.Contains("Don't Starve Together Dedicated Server") && !fileName.Contains("饥荒联机版专用服务器"))
                {
                    if (MessageBox.Show("似乎选择了客户端目录的程序，请确认！如果确定没有问题仍然保存点击“是”(判断出错的情况一般只出现在WeGame版)！", "似乎选错了呢...", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        return;
                }
                CommonPath.ServerFilePath = fileName;
                DediDirSelectTextBox.Text = fileName;
                CommonPath.WriteServerPath(fileName, CommonPath.GamePlatform);
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }

        /// <summary>
        /// 保存ClusterToken
        /// </summary>
        private void SaveClusterToken(object sender, RoutedEventArgs e)
        {
            var clusterToken = DediSettingClusterTokenTextBox.Text.Trim();
            if (string.IsNullOrEmpty(clusterToken))
            {
                MessageBox.Show("clusterToken没填写，不能保存");
            }
            else
            {
                // 判断ClusterToken有效性
                if (clusterToken.Length < 6)
                {
                    MessageBox.Show("clusterToken过短，不能保存");
                    return;
                }
                var flag = clusterToken.Length != 32;
                if (clusterToken.Substring(0, 6) == "pds-g^" && clusterToken.Length == 62)
                {
                    flag = false;

                }
                if (flag)
                {
                    if (MessageBox.Show("clusterToken格式不正确，确定依然要保存吗？", "出错了唉",
                            MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }
                // 确定有效，保存ClusterToken↓
                CommonPath.ClusterToken = clusterToken;
                MessageBox.Show("保存完毕！");
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }

        #endregion
    }
}
