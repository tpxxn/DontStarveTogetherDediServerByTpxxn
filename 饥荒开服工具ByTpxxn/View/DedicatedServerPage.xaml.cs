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
        private Dictionary<string, string> _Hanization;  // 汉化
        private PathFile _pathFile;                      // 文件路径
        private BaseSet _baseSet;                        // 基本设置
        private Leveldataoverride _overWorld;            // 地上世界
        private Leveldataoverride _caves;                // 地下世界
        private Mods _mods;                              // mods
        private int SaveSlot { get; set; }               // 存档槽
        private readonly UTF8Encoding _utf8WithoutBom = new UTF8Encoding(false); // 编码
        #endregion

        #region 构造事件及初始化
        /// <summary>
        /// 构造事件
        /// </summary>
        public DedicatedServerPage()
        {
            InitializeComponent();
            // 初始化左侧选择存档RadioButton的Tag
            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{i}")).Tag = i;
            }
            // 初始化服务器面板
            PanelInitalize();
        }

        /// <summary>
        /// 面板初始化
        /// </summary>
        private void PanelInitalize()
        {
            // 隐藏所有面板
            PanelVisibility("Null");
            #region 设置PathCommon类数据
            // 服务器版本[Steam/WeGame]
            GamePlatformSelectBox.TextList = new List<string> { "Steam", "WeGame" };
            // 检查通用设置
            CheckCommonSetting();
            #endregion
            #region ComboBox初始化
            BaseSetGameModeSelectBox.TextList = new List<string> { "生存", "荒野", "无尽" };
            BaseSetPvpSelectBox.TextList = new List<string> { "否", "是" };
            var maxPlayer = new List<string>();
            for (var i = 1; i <= 64; i++)
            {
                maxPlayer.Add(i.ToString());
            }
            BaseSetMaxPlayerSelectBox.TextList = maxPlayer;
            BaseSetOfflineSelectBox.TextList = new List<string> { "在线", "离线" };
            BaseSetIsPauseSelectBox.TextList = new List<string> { "否", "是" }; ;
            EditWorldIsCaveSelectBox.TextList = new List<string> { "否", "是" }; ;
            #endregion
            // 初始化
            InitServer();
            // 显示通用设置面板
            PanelVisibility("CommonSetting");
        }
        #endregion

        #region "面板按钮"
        /// <summary>
        /// 显示指定面板/隐藏所有面板[Null]
        /// </summary>
        /// <param name="str">要显示的面板</param>
        private void PanelVisibility(string str)
        {
            foreach (UIElement vControl in CenterMainGrid.Children)
            {
                vControl.Visibility = Visibility.Collapsed;
            }
            switch (str)
            {
                case "Null":
                    // 隐藏所有面板
                    break;
                case "CommonSetting":
                    DediCommonSetting.Visibility = Visibility.Visible;
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

        #region LeftPanel按钮

        /// <summary>
        /// 左侧SaveSlotRadioButton Click事件
        /// </summary>
        private void SaveSlotRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // 存档
            SaveSlot = int.Parse(((RadioButton)sender).Name.Remove(0, 19));
            // -1.保存之前的
            if (_overWorld != null && _caves != null && _mods != null && Directory.Exists(_pathFile.ServerDirPath))
            {
                // 地面和洞穴世界保存
                _overWorld.SaveWorld();
                _caves.SaveWorld();
                // 地面和洞穴MOD保存
                _mods.SaveListmodsToFile(_pathFile.ServerDirPath + @"\Master\modoverrides.lua", _utf8WithoutBom);
                _mods.SaveListmodsToFile(_pathFile.ServerDirPath + @"\Caves\modoverrides.lua", _utf8WithoutBom);
            }
            // 1.复制文件
            if (((RadioButton)sender).Content.ToString() == "新世界")
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
            InitServer(true);
            // 3 禁用
            Forbidden(false);
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

        #endregion

        #region Top按钮

        private void TitleMenuBaseSet_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("BaseSet");
        }

        private void TitleMenuEditWorld_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("EditWorld");
        }

        private void TitleMenuMod_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("Mod");
        }

        private void TitleMenuRollback_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("Rollback");
        }

        //TODO 黑名单列表
        //private void DediTitleBlacklist_Click(object sender, RoutedEventArgs e)
        //{
        //    PanelVisibility("Blacklist");
        //}

        #endregion

        #region Bottom按钮

        /// <summary>
        /// 通用设置
        /// </summary>
        private void CommonSettingButton_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("CommonSetting");
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
                    // 目录,这个必须设置
                    WorkingDirectory = Path.GetDirectoryName(PathCommon.ClientFilePath) ?? throw new InvalidOperationException(),
                    FileName = PathCommon.ClientFilePath
                }
            };
            process.Start();
        }

        /// <summary>
        /// 删除档位
        /// </summary>
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
            ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).Content = "新世界";
            // 2. 删除当前存档
            if (Directory.Exists(_pathFile.ServerDirPath))
            {
                Directory.Delete(_pathFile.ServerDirPath, true);
            }
            // 2.1 取消选择,谁都不选
            // ReSharper disable once PossibleNullReferenceException
            ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{SaveSlot}")).IsChecked = false;
            // 2.2 
            // DediMainBorder.IsEnabled = false;
            Forbidden(true);
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
        /// 打开服务器
        /// </summary>
        private void RunServer()
        {
            if (string.IsNullOrEmpty(PathCommon.ServerFilePath))
            {
                MessageBox.Show("服务器路径不对,请重新设置服务器路径");
                return;
            }
            // 保存世界
            if (_overWorld != null && _caves != null && _mods != null)
            {
                // TODO 没有等待？
                _overWorld.SaveWorld();
                _caves.SaveWorld();
                _mods.SaveListmodsToFile(_pathFile.ServerDirPath + @"\Master\modoverrides.lua", _utf8WithoutBom);
                _mods.SaveListmodsToFile(_pathFile.ServerDirPath + @"\Caves\modoverrides.lua", _utf8WithoutBom);
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
                        UseShellExecute = false,
                        // 目录,这个必须设置
                        WorkingDirectory = Path.GetDirectoryName(PathCommon.ServerFilePath) ?? throw new InvalidOperationException(), 
                        // 服务器名字
                        FileName = PathCommon.ServerFilePath,
                        Arguments = "-console -cluster DedicatedServer_" + SaveSlot +" -shard Master"
                    }
                };
                masterProcess.Start();
                // 打开服务器(洞穴)
                if (EditWorldIsCaveSelectBox.Text == "是")
                {
                    var caveProcess = new Process
                    {
                        StartInfo =
                        {
                            UseShellExecute = false,
                            // 目录,这个必须设置
                            WorkingDirectory = Path.GetDirectoryName(PathCommon.ServerFilePath) ?? throw new InvalidOperationException(), 
                            // 服务器名字
                            FileName = PathCommon.ServerFilePath,
                            Arguments = "-console -cluster DedicatedServer_" + SaveSlot +" -shard Caves"
                        }
                    };
                    caveProcess.Start();
                }
            }
        }

        #endregion

        #endregion

        #region 其他方法

        /// <summary>
        /// 获取房间名
        /// </summary>
        /// <param name="saveSlot">存档槽</param>
        /// <returns>房间名</returns>
        private string GetHouseName(int saveSlot)
        {
            var clusterIniPath = PathCommon.SaveRootDirPath + @"\DedicatedServer_" + saveSlot + @"\cluster.ini";
            if (!File.Exists(clusterIniPath))
            {
                return "新世界";
            }
            var iniTool = new IniHelper(clusterIniPath, _utf8WithoutBom);
            var houseName = iniTool.ReadValue("NETWORK", "cluster_name");
            return houseName;
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public void InitServer(bool createWorld = false)
        {
            // 1.游戏平台
            PathCommon.GamePlatform = PathCommon.ReadGamePlatform();
            switch (PathCommon.GamePlatform)
            {
                case "Steam":
                    GamePlatformSelectBox.TextIndex = 0;
                    break;
                case "WeGame":
                    GamePlatformSelectBox.TextIndex = 1;
                    break;
            }
            // 2.检查存档Server是否存在 
            CheckServer();
            // 3.汉化
            _Hanization = JsonHelper.ReadHanization();
            // 4.控制台
            CreateConsoleClassificationButton();
            #region 仅创建世界时
            // 5.[创建世界]设定文件路径
            if (createWorld)
            {
                _pathFile = new PathFile(SaveSlot);
            }
            // 6.[创建世界]读取服务器mods文件夹下所有信息.mod多的话,读取时间也多
            //   此时的mod没有被current覆盖
            _mods = null;
            if (!string.IsNullOrEmpty(PathCommon.ServerModsDirPath) && createWorld)
            {
                _mods = new Mods(PathCommon.ServerModsDirPath);
                SetModSet();
            }
            #endregion
        }

        /// <summary>
        /// "检查"服务器目录
        /// </summary>
        private void CheckServer()
        {
            // 判断是否存在存档根目录文件夹
            if (!Directory.Exists(PathCommon.SaveRootDirPath))
            {
                Directory.CreateDirectory(PathCommon.SaveRootDirPath);
            }
            // 获取服务器路径列表
            var directoryInfos = new DirectoryInfo(PathCommon.SaveRootDirPath).GetDirectories();
            var serverPathList = (from directoryInfo in directoryInfos where directoryInfo.Name.StartsWith("DedicatedServer_") select directoryInfo.FullName).ToList();
            // 清空左边
            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.FindName($"SaveSlotRadioButton{i}")).Content = "新世界";
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
            Forbidden(true);
        }

        #region 汉化

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

        #endregion

        /// <summary>
        /// 禁用菜单[基本设置|编辑世界|Mod|控制台|删除存档|创建世界]
        /// </summary>
        /// <param name="isDisable">是否禁用</param>
        private void Forbidden(bool isDisable)
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
