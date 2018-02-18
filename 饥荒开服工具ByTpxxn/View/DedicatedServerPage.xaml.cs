using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml;
using Newtonsoft.Json;
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;

namespace 饥荒开服工具ByTpxxn.View
{
    /// <summary>
    /// DedicatedServerPage.xaml 的交互逻辑
    /// </summary>
    public partial class DedicatedServerPage : Page
    {
        #region 字段、属性
        private readonly UTF8Encoding _utf8WithoutBom = new UTF8Encoding(false); // 编码
        private Dictionary<string, string> _Hanization;  // 汉化
        private PathAll _pathAll;                        // 路径
        private BaseSet _baseSet;                        // 基本设置
        private Leveldataoverride _overWorld;            // 地上世界
        private Leveldataoverride _caves;                // 地下世界
        private Mods _mods;                              // mods
        private bool _firstLoad = true;                  // 首次加载
        private int SaveSlot { get; set; }               // 存档槽
        #endregion

        #region 构造事件及初始化
        /// <summary>
        /// 构造事件
        /// </summary>
        public DedicatedServerPage()
        {
            InitializeComponent();
            //初始化左侧选择存档RadioButton的Tag
            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{i}")).Tag = i;
            }
            // 初始化服务器面板
            DedicatedServerPanelInitalize();
        }

        /// <summary>
        /// 初始化服务器面板
        /// </summary>
        private void DedicatedServerPanelInitalize()
        {
            // 隐藏所有面板
            DediButtomPanelVisibilityInitialize();
            #region ComboBox初始化
            string[] noYes = { "否", "是" };
            var maxPlayer = new string[64];
            for (var i = 1; i <= 64; i++)
            {
                maxPlayer[i - 1] = i.ToString();
            }
            DediBaseSetGamemodeSelect.ItemsSource = new[] { "生存", "荒野", "无尽" };
            DediBaseSetPvpSelect.ItemsSource = noYes;
            DediBaseSetMaxPlayerSelect.ItemsSource = maxPlayer;
            DediBaseOfflineSelect.ItemsSource = new[] { "在线", "离线" };
            DediBaseIsPause.ItemsSource = noYes;
            IsCaveComboBox.ItemsSource = noYes;
            #endregion
            #region 设置PathCommon类数据
            // 服务器版本[Steam/WeGame]
            DediSettingGameVersionSelect.TextList = new List<string> { "Steam", "WeGame" };
            // 检查通用设置
            CheckCommonSetting();
            #endregion
            // 初始化
            InitServer();
            // 显示通用设置面板
            DediButtomPanelVisibility("Setting");
        }
        #endregion

        #region 主面板菜单

        #region "面板菜单按钮"
        private void TitleMenuBaseSet_Click(object sender, RoutedEventArgs e)
        {
            DediButtomPanelVisibility("BaseSet");
        }

        private void TitleMenuEditWorld_Click(object sender, RoutedEventArgs e)
        {
            DediButtomPanelVisibility("EditWorld");
        }

        private void TitleMenuMod_Click(object sender, RoutedEventArgs e)
        {
            DediButtomPanelVisibility("Mod");
        }

        private void TitleMenuRollback_Click(object sender, RoutedEventArgs e)
        {
            DediButtomPanelVisibility("Rollback");
        }

        //TODO 黑名单列表
        //private void DediTitleBlacklist_Click(object sender, RoutedEventArgs e)
        //{
        //    DediButtomPanelVisibility("Blacklist");
        //}

        private void DeleteWorldButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 0. 关闭服务器
            var processes = Process.GetProcesses();
            foreach (var item in processes)
            {
                if (item.ProcessName == "dontstarve_dedicated_server_nullrenderer")
                {
                    item.Kill();
                }
            }
            // 1. radioBox 写 创建世界
            // ReSharper disable once PossibleNullReferenceException
            ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).Content = "创建世界";
            // 2. 删除当前存档
            if (Directory.Exists(_pathAll.ServerDirPath))
            {
                Directory.Delete(_pathAll.ServerDirPath, true);
            }
            // 2.1 取消选择,谁都不选
            // ReSharper disable once PossibleNullReferenceException
            ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).IsChecked = false;
            // 2.2 
            // DediMainBorder.IsEnabled = false;
            JinYong(true);
            //// 3. 复制一份新的过来                 
            //ServerTools.FileHelper.CopyDirectory(pathAll.ServerMoBanPath, pathAll.DoNotStarveTogether_DirPath);
            //if (!Directory.Exists(pathAll.DoNotStarveTogether_DirPath + "\\Server_" + PathCommon.GamePlatform + "_" + SaveSlot))
            //{
            //    Directory.Move(pathAll.DoNotStarveTogether_DirPath + "\\Server", pathAll.DoNotStarveTogether_DirPath + "\\Server_" + PathCommon.GamePlatform + "_" + SaveSlot);
            //}
            //// 4. 读取新的存档
            //SetBaseSet();
        }

        /// <summary>
        /// 通用设置
        /// </summary>
        private void CommonSettingButton_Click(object sender, RoutedEventArgs e)
        {
            DediButtomPanelVisibility("Setting");
        }

        /// <summary>
        /// 打开客户端
        /// </summary>
        private static void RunClient()
        {
            if (string.IsNullOrEmpty(PathCommon.ClientModsDirPath))
            {
                MessageBox.Show("客户端路径没有设置");
                return;
            }
            var process = new Process
            {
                StartInfo =
                {
                    Arguments = "",
                    WorkingDirectory = Path.GetDirectoryName(PathCommon.ClientFilePath) ?? throw new InvalidOperationException(),
                    FileName = PathCommon.ClientFilePath
                }
            };
            // 目录,这个必须设置
            process.Start();
        }

        /// <summary>
        /// 打开服务器
        /// </summary>
        private void RunServer()
        {
            if (PathCommon.ServerFilePath == null || PathCommon.ServerFilePath.Trim() == "")
            {
                MessageBox.Show("服务器路径不对,请重新设置服务器路径"); return;
            }
            // 保存世界
            if (_overWorld != null && _caves != null && _mods != null)
            {
                _overWorld.SaveWorld();
                _caves.SaveWorld();
                _mods.SaveListmodsToFile(_pathAll.ServerDirPath + @"\Master\modoverrides.lua", _utf8WithoutBom);
                _mods.SaveListmodsToFile(_pathAll.ServerDirPath + @"\Caves\modoverrides.lua", _utf8WithoutBom);
            }
            // 服务器启动
            if (PathCommon.GamePlatform == "WeGame")
            {
                MessageBox.Show("保存完毕! 请通过WeGame启动,存档文件名为" + PathCommon.GamePlatform + "_" + SaveSlot);
            }
            else
            {
                // 打开服务器(地面)
                var masterProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false, // 是否
                        WorkingDirectory = Path.GetDirectoryName(PathCommon.ServerFilePath) ?? throw new InvalidOperationException(), // 目录,这个必须设置
                        FileName = PathCommon.ServerFilePath, // 服务器名字
                        Arguments = "-console -cluster Server_" + PathCommon.GamePlatform + "_" + SaveSlot +" -shard Master"
                    }
                };
                masterProcess.Start();
                // 打开服务器(洞穴)
                if (IsCaveComboBox.Text == "是")
                {
                    var caveProcess = new Process
                    {
                        StartInfo =
                        {
                            UseShellExecute = false, // 是否
                            WorkingDirectory = Path.GetDirectoryName(PathCommon.ServerFilePath) ?? throw new InvalidOperationException(), // 目录,这个必须设置
                            FileName = PathCommon.ServerFilePath, // 服务器名字
                            Arguments = "-console -cluster Server_" + PathCommon.GamePlatform + "_" + SaveSlot +" -shard Caves"
                        }
                    };
                    caveProcess.Start();
                }
            }
        }
        #endregion

        #region "主面板Visibility属性设置"
        /// <summary>
        /// 隐藏所有面板
        /// </summary>
        private void DediButtomPanelVisibilityInitialize()
        {
            foreach (UIElement vControl in CenterMainGrid.Children)
            {
                vControl.Visibility = Visibility.Collapsed;
            }
        }

        // 显示指定面板
        private void DediButtomPanelVisibility(string obj)
        {
            DediButtomPanelVisibilityInitialize();
            switch (obj)
            {
                case "Setting":
                    DediSetting.Visibility = Visibility.Visible;
                    TitleMenuBaseSet.IsChecked = false;
                    TitleMenuEditWorld.IsChecked = false;
                    TitleMenuMod.IsChecked = false;
                    TitleMenuRollback.IsChecked = false;
                    break;
                case "BaseSet":
                    DediBaseSet.Visibility = Visibility.Visible;
                    break;
                case "EditWorld":
                    DediWorldSet.Visibility = Visibility.Visible;
                    break;
                case "Mod":
                    DediModSet.Visibility = Visibility.Visible;
                    break;
                case "Rollback":
                    DediConsole.Visibility = Visibility.Visible;
                    break;
                case "Blacklist":
                    DediModManager.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #endregion

        #region "通用设置面板"

        /// <summary>
        /// 游戏平台改变,初始化一切
        /// </summary>
        private void DediSettingGameVersionSelect_SelectionChanged()
        {
            // 赋值
            PathCommon.GamePlatform = DediSettingGameVersionSelect.TextList[DediSettingGameVersionSelect.TextIndex];
            if (PathCommon.GamePlatform == "WeGame")
            {
                CtrateRunGame.Visibility = Visibility.Collapsed;
                CtrateWorldButton.Content = "保存世界";
            }
            else
            {
                CtrateRunGame.Visibility = Visibility.Visible;
                CtrateWorldButton.Content = "创建世界";
            }
            // 初始化
            InitServer();
        }

        /// <summary>
        /// 左侧SaveSlotRadioButton Click事件
        /// </summary>
        private void SaveSlotRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // 存档
            SaveSlot = int.Parse(((RadioButton)sender).Name.Remove(0, 19));
            // -1.保存之前的
            if (_overWorld != null && _caves != null && _mods != null && Directory.Exists(_pathAll.ServerDirPath))
            {
                // 地面和洞穴世界保存
                _overWorld.SaveWorld();
                _caves.SaveWorld();
                // 地面和洞穴MOD保存
                _mods.SaveListmodsToFile(_pathAll.ServerDirPath + @"\Master\modoverrides.lua", _utf8WithoutBom);
                _mods.SaveListmodsToFile(_pathAll.ServerDirPath + @"\Caves\modoverrides.lua", _utf8WithoutBom);
            }
            // 1.复制文件
            if (((RadioButton)sender).Content.ToString() == "创建世界")
            {
                // 复制一份过去                  
                CopyServerFile();
                // 改名字
                if (!Directory.Exists(PathCommon.SaveRootDirPath + @"\DedicatedServer_" + SaveSlot))
                {
                    Directory.Move(PathCommon.SaveRootDirPath + @"\Server", PathCommon.SaveRootDirPath + @"\DedicatedServer_" + SaveSlot);
                    // 删除临时文件
                    if (Directory.Exists(PathCommon.SaveRootDirPath + @"\Server"))
                    {
                        Directory.Delete(PathCommon.SaveRootDirPath + @"\Server", true);
                    }
                }
                ((RadioButton)sender).Content = GetHouseName(SaveSlot);
            }
            // 2.初始化服务器
            InitServer();
            // 3 禁用
            JinYong(false);
            // 4.读取并设定基本设置
            SetBaseSet();
            // 5. "地面世界设置"
            SetOverWorldSet();
            // 5. "洞穴世界设置"
            SetCavesSet();
            // 6.打开基本设置页面
            TitleMenuBaseSet.IsChecked = true;
            TitleMenuBaseSet_Click(null, null);
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public void InitServer()
        {
            //-1.游戏平台
            PathCommon.GamePlatform = PathCommon.ReadGamePlatform();
            switch (PathCommon.GamePlatform)
            {
                case "Steam":
                    DediSettingGameVersionSelect.TextIndex = 0;
                    break;
                case "WeGame":
                    DediSettingGameVersionSelect.TextIndex = 1;
                    break;
            }
            // 0.路径信息
            if (_firstLoad == false)
            {
                SetPath();
            }
            // 1.检查存档Server是否存在 
            CheckServer();
            // 2.汉化
            _Hanization = JsonHelper.ReadHanization();
            // 3.读取服务器mods文件夹下所有信息.mod多的话,读取时间也多
            //   此时的mod没有被current覆盖
            _mods = null;
            if (!string.IsNullOrEmpty(PathCommon.ServerModsDirPath) && _firstLoad == false)
            {
                _mods = new Mods(PathCommon.ServerModsDirPath);
                SetModSet();
            }
            // 4. "控制台"
            CreateConsoleClassificationButton();
            // 5.如果_firstLoad为真则设置为假
            if (_firstLoad)
                _firstLoad = false;
        }

        /// <summary>
        /// 设置"路径"
        /// </summary>
        private void SetPath()
        {
            _pathAll = new PathAll(SaveSlot);
            PathCommon.GamePlatform = PathCommon.ReadGamePlatform();
            // 客户端路径
            GameDirSelectTextBox.Text = "";
            PathCommon.ClientFilePath = PathCommon.ReadClientPath(PathCommon.GamePlatform);
            if (!string.IsNullOrEmpty(PathCommon.ClientFilePath) && File.Exists(PathCommon.ClientFilePath))
            {
                GameDirSelectTextBox.Text = PathCommon.ClientFilePath;
            }
            else
            {
                PathCommon.ClientFilePath = "";
            }
            // 服务器路径
            DediDirSelectTextBox.Text = "";
            PathCommon.ServerFilePath = PathCommon.ReadServerPath(PathCommon.GamePlatform);
            if (!string.IsNullOrEmpty(PathCommon.ServerFilePath) && File.Exists(PathCommon.ServerFilePath))
            {
                DediDirSelectTextBox.Text = PathCommon.ServerFilePath;
            }
            else
            {
                PathCommon.ServerFilePath = "";
            }
            // ClusterToken
            DediSettingClusterTokenTextBox.Text = "";
            PathCommon.ClusterToken = PathCommon.ReadClusterTokenPath(PathCommon.GamePlatform);
            if (!string.IsNullOrEmpty(PathCommon.ClusterToken))
            {
                DediSettingClusterTokenTextBox.Text = PathCommon.ClusterToken;
            }
            else
            {
                PathCommon.ClusterToken = "";
            }
        }

        /// <summary>
        /// "检查"服务器目录
        /// </summary>
        private void CheckServer()
        {
            if (!Directory.Exists(PathCommon.SaveRootDirPath))
            {
                Directory.CreateDirectory(PathCommon.SaveRootDirPath);
            }
            var directoryInfos = new DirectoryInfo(PathCommon.SaveRootDirPath).GetDirectories();
            var serverPathList = (from directoryInfo in directoryInfos where directoryInfo.Name.StartsWith("Server_" + PathCommon.GamePlatform + "_") select directoryInfo.FullName).ToList();
            // 清空左边
            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{i}")).Content = "创建世界";
            }
            // 等于0
            if (serverPathList.Count != 0)
            {
                //    // 复制一份过去                  
                //    //FileHelper.CopyDirectory(pathAll.ServerMoBanPath, pathAll.DoNotStarveTogether_DirPath);
                //    CopyServerModel(PathCommon.SaveRootDirPath);
                //    // 改名字
                //    if (!Directory.Exists(PathCommon.SaveRootDirPath + @"\Server_" + PathCommon.GamePlatform + "_0"))
                //    {
                //        Directory.Move(PathCommon.SaveRootDirPath + @"\Server", PathCommon.SaveRootDirPath + @"\Server_" + PathCommon.GamePlatform + "_0");
                //    }
                //}
                //else
                //{
                foreach (var serverPath in serverPathList)
                {
                    // 取出序号 
                    var num = serverPath.Substring(serverPath.LastIndexOf('_') + 1);
                    // 取出存档名称
                    // ReSharper disable once PossibleNullReferenceException
                    ((RadioButton)SaveSlotStackPanel.FindName("SaveSlotRadioButton" + num)).Content = GetHouseName(int.Parse(num));
                }
            }
            // 禁用
            JinYong(true);
        }

        /// <summary>
        /// 复制ServerTemplate到指定位置
        /// </summary>
        private void CopyServerFile()
        {
            // 判断是否存在
            if (Directory.Exists(PathCommon.SaveRootDirPath + @"\Server"))
            {
                Directory.Delete(PathCommon.SaveRootDirPath + @"\Server", true);
            }
            // 建立文件夹
            Directory.CreateDirectory(PathCommon.SaveRootDirPath + @"\Server");
            Directory.CreateDirectory(PathCommon.SaveRootDirPath + @"\Server\Caves");
            Directory.CreateDirectory(PathCommon.SaveRootDirPath + @"\Server\Master");
            // 填文件
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\cluster.ini", FileHelper.ReadResources("ServerTemplate.cluster.ini"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Caves\leveldataoverride.lua", FileHelper.ReadResources("ServerTemplate.Caves.leveldataoverride.lua"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Caves\modoverrides.lua", FileHelper.ReadResources("ServerTemplate.Caves.modoverrides.lua"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Caves\server.ini", FileHelper.ReadResources("ServerTemplate.Caves.server.ini"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Master\leveldataoverride.lua", FileHelper.ReadResources("ServerTemplate.Master.leveldataoverride.lua"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Master\modoverrides.lua", FileHelper.ReadResources("ServerTemplate.Master.modoverrides.lua"), _utf8WithoutBom);
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\Master\server.ini", FileHelper.ReadResources("ServerTemplate.Master.server.ini"), _utf8WithoutBom);
            // ClusterToken
            File.WriteAllText(PathCommon.SaveRootDirPath + @"\Server\cluster_token.txt",
                !string.IsNullOrEmpty(PathCommon.ClusterToken) 
                ? PathCommon.ClusterToken
                : "",
                _utf8WithoutBom);
        }

        /// <summary>
        /// 选择游戏exe文件
        /// </summary>
        private void DediSettingGameDirSelect_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择游戏exe文件",
                FileName = PathCommon.GamePlatform == "WeGame"
                    ? "dontstarve_rail"
                    : "dontstarve_steam", //默认文件名
                DefaultExt = ".exe",// 默认文件扩展名
                Filter = PathCommon.GamePlatform == "WeGame"
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
                PathCommon.ClientFilePath = fileName;
                GameDirSelectTextBox.Text = fileName;
                PathCommon.WriteClientPath(fileName, PathCommon.GamePlatform);
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }

        /// <summary>
        /// 选择服务器文件
        /// </summary>
        private void DediSettingDediDirSelect_Click(object sender, RoutedEventArgs e)
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
                PathCommon.ServerFilePath = fileName;
                DediDirSelectTextBox.Text = fileName;
                PathCommon.WriteServerPath(fileName, PathCommon.GamePlatform);
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }

        /// <summary>
        /// 双击打开所在文件夹"客户端"
        /// </summary>
        private void DediSettingGameDirSelectTextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(PathCommon.ClientFilePath) && File.Exists(PathCommon.ClientFilePath))
            {
                Process.Start(Path.GetDirectoryName(PathCommon.ClientFilePath) ?? throw new InvalidOperationException());
            }
        }

        /// <summary>
        /// 双击打开所在文件夹"服务端"
        /// </summary>
        private void DediSettingDediDirSelectTextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(PathCommon.ServerFilePath) && File.Exists(PathCommon.ServerFilePath))
            {
                Process.Start(Path.GetDirectoryName(PathCommon.ServerFilePath) ?? throw new InvalidOperationException());
            }
        }

        /// <summary>
        /// 保存ClusterToken
        /// </summary>
        private void DediSettingSaveCluster_Click(object sender, RoutedEventArgs e)
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
                // 确定有效↓
                PathCommon.ClusterToken = clusterToken;
                PathCommon.WriteClusterTokenPath(clusterToken, PathCommon.ReadGamePlatform());
                MessageBox.Show("保存完毕！");
                // 检查通用设置
                CheckCommonSetting(true);
            }
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 汉化[字符串]
        /// </summary>
        /// <param name="str">字符串str</param>
        /// <returns>汉化文本</returns>
        private string Hanization(string str)
        {
            return _Hanization.ContainsKey(str) ? _Hanization[str] : str;
        }

        /// <summary>
        /// 汉化[字符串公开枚举数]
        /// </summary>
        /// <param name="str">字符串str</param>
        /// <returns>汉化文本[List]</returns>
        private IEnumerable<string> Hanization(IEnumerable<string> str)
        {
            return str.Select(item => _Hanization.ContainsKey(item) ? _Hanization[item] : item).ToList();
        }

        /// <summary>
        /// 获取房间名
        /// </summary>
        /// <param name="saveSlot">存档槽</param>
        /// <returns>房间名</returns>
        private string GetHouseName(int saveSlot)
        {
            var clusterIniPath = PathCommon.SaveRootDirPath + @"\Server_" + PathCommon.GamePlatform + "_" + saveSlot + @"\cluster.ini";
            if (!File.Exists(clusterIniPath))
            {
                return "创建世界";
            }
            var iniTool = new IniHelper(clusterIniPath, _utf8WithoutBom);

            var houseName = iniTool.ReadValue("NETWORK", "cluster_name");
            return houseName;
        }

        /// <summary>
        /// 检查通用设置
        /// </summary>
        /// <param name="onCommonSettingPanel">是否已经在通用设置面板</param>
        private void CheckCommonSetting(bool onCommonSettingPanel = false)
        {
            // 读取通用设置
            SetPath();
            if (string.IsNullOrEmpty(PathCommon.ClientFilePath) || string.IsNullOrEmpty(PathCommon.ServerFilePath) || string.IsNullOrEmpty(PathCommon.ClusterToken))
            {
                if (onCommonSettingPanel == false)
                {
                    CommonSettingSetOverTextBlock.Text = "请先设定好通用设置！";
                    CommonSettingSetOverTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
                JinYong(true);
                SaveSlotLeftPanelCenterScrollViewer.IsEnabled = false;
                DediButtomPanelVisibility("Setting");
            }
            else
            {
                SaveSlotLeftPanelCenterScrollViewer.IsEnabled = true;
                CommonSettingSetOverTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                CommonSettingSetOverTextBlock.Text = "通用设置设定完毕，现在可以在左侧选择存档开启服务器";
            }
        }

        /// <summary>
        /// 禁用菜单
        /// </summary>
        /// <param name="isDisable">是否禁用</param>
        private void JinYong(bool isDisable)
        {
            // 基本设置
            TitleMenuBaseSet.IsEnabled = !isDisable;
            // 编辑世界
            TitleMenuEditWorld.IsEnabled = !isDisable;
            // Mod
            TitleMenuMod.IsEnabled = !isDisable;
            // 控制台
            TitleMenuRollback.IsEnabled = !isDisable;
            // 删除存档按钮
            DeleteWorldButton.IsEnabled = !isDisable;
            // 创建世界按钮
            CtrateWorldButton.IsEnabled = !isDisable;
        }
        #endregion

    }
}
