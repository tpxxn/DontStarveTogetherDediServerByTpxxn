using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        private DediFilePath _dediFilePath;              // 文件路径

        private BaseSet _baseSet;                        // 基本设置

        private HanizationObject _Hanization;            // 汉化
        private Leveldataoverride _overWorld;            // 地上世界
        private Leveldataoverride _caves;                // 地下世界

        private Mods _mods;                              // mods

        private int SaveSlot { get; set; }               // 存档槽

        #endregion

        #region 构造事件及初始化
        /// <summary>
        /// 构造事件
        /// </summary>
        public DedicatedServerPage()
        {
            InitializeComponent();
            Global.DedicatedServerFrame.NavigationService.LoadCompleted += LoadCompleted;
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            // 初始化左侧选择存档RadioButton的Tag
            for (var i = 0; i < SaveSlotStackPanel.Children.Count; i++)
            {
                ((RadioButton)SaveSlotStackPanel.Children[i]).Tag = i;
            }
            // 初始化服务器面板
            PanelInitalize();
        }

        /// <summary>
        /// 面板初始化
        /// </summary>
        private async void PanelInitalize()
        {
            // 隐藏所有面板
            PanelVisibility("Null");
            #region 设置PathCommon类数据
            // 服务器版本[Steam/WeGame]
            GamePlatformSelectBox.TextList = new List<string> { "Steam", "WeGame" };
            // 检查通用设置
            CheckCommonSetting();
            #endregion
            #region BaseSet初始化
            BaseSetGameModeSelectBox.TextList = new List<string> { "生存", "荒野", "无尽" };
            BaseSetPvpSelectBox.TextList = new List<string> { "关闭", "开启" };
            var maxPlayer = new List<string>();
            for (var i = 1; i <= 64; i++)
            {
                maxPlayer.Add(i.ToString());
            }
            BaseSetMaxPlayerSelectBox.TextList = maxPlayer;
            BaseSetOfflineSelectBox.TextList = new List<string> { "在线", "离线" };
            BaseSetIsPauseSelectBox.TextList = new List<string> { "关闭", "开启" }; ;
            EditWorldIsCaveSelectBox.TextList = new List<string> { "关闭", "开启" }; ;
            #endregion
            #region Mod初始化
            // 此时的mod没有被current覆盖
            _mods = null;
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath))
            {
                var dialogWindow = new DialogWindow("mod加载中") { Owner = Application.Current.MainWindow };
                dialogWindow.InitializeComponent();
                MainGrid.IsEnabled = false;
                dialogWindow.Show();
                _mods = await Task.Run(() => new Mods());
                MainGrid.IsEnabled = true;
                dialogWindow.Close();
            }
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
            foreach (UIElement vControl in SettingMainGrid.Children)
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
            SaveSlot = (int)((RadioButton)sender).Tag;
            // [若地面、洞穴、mod对象存在且服务器文件夹存在]保存之前的
            if (_overWorld != null && _caves != null && _mods != null && Directory.Exists(_dediFilePath.ServerDirPath))
            {
                // 地面和洞穴世界保存
                _overWorld.SaveWorld();
                _caves.SaveWorld();
                // 地面和洞穴MOD保存
                _mods.SaveListmodsToFile(_dediFilePath.ModConfigOverworldFilePath, Global.Utf8WithoutBom);
                _mods.SaveListmodsToFile(_dediFilePath.ModConfigCaveFilePath, Global.Utf8WithoutBom);
            }
            // 复制文件
            if (((RadioButton)SaveSlotStackPanel.Children[SaveSlot]).Content.ToString() == "新世界")
            {
                // 复制一份过去                  
                FileHelper.CopyServerTemplateFile();
                // 改名字
                FileHelper.RenameServerTemplateFile(SaveSlot);
                // 修改按钮Content为房间名
                ((RadioButton)SaveSlotStackPanel.Children[SaveSlot]).Content = GetClusterName(SaveSlot);
            }
            // 初始化服务器
            InitServer(true);
            // 禁用
            Forbidden(false);
            // 读取并设定基本设置
            SetBaseSet();
            // "地面世界设置"
            SetOverWorldSet();
            // "洞穴世界设置"
            SetCavesSet();
            // 打开基本设置页面
            TitleMenuBaseSet.IsChecked = true;
            TitleMenuBaseSet_Click(null, null);
        }

        #endregion

        #region SettingTitle按钮

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

        #region SettingFooter按钮

        /// <summary>
        /// 通用设置
        /// </summary>
        private void CommonSettingButton_Click(object sender, RoutedEventArgs e)
        {
            PanelVisibility("CommonSetting");
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            RunClient();
        }

        /// <summary>
        /// 打开客户端
        /// </summary>
        private static void RunClient()
        {
            if (string.IsNullOrEmpty(CommonPath.ClientModsDirPath))
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
                    WorkingDirectory = Path.GetDirectoryName(CommonPath.ClientFilePath) ?? throw new InvalidOperationException(),
                    FileName = CommonPath.ClientFilePath
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
            ((RadioButton)SaveSlotStackPanel.Children[SaveSlot]).Content = "新世界";
            // 2. 删除当前存档
            if (Directory.Exists(_dediFilePath.ServerDirPath))
            {
                Directory.Delete(_dediFilePath.ServerDirPath, true);
            }
            // 2.1 取消选择,谁都不选
            var radioButtonList = new List<RadioButton>();
            Global.FindChildren(radioButtonList, SaveSlotLeftPanelCenterScrollViewer);
            radioButtonList[SaveSlot].IsChecked = false;
            // 2.2 
            PanelVisibility("CommonSetting");
            Forbidden(true);
        }

        /// <summary>
        /// 创建世界按钮
        /// </summary>
        private void CtrateWorldButton_Click(object sender, RoutedEventArgs e)
        {
            RunServer();
        }

        /// <summary>
        /// 打开服务器
        /// </summary>
        private void RunServer()
        {
            if (string.IsNullOrEmpty(CommonPath.ServerFilePath))
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
                _mods.SaveListmodsToFile(_dediFilePath.ModConfigOverworldFilePath, Global.Utf8WithoutBom);
                _mods.SaveListmodsToFile(_dediFilePath.ModConfigCaveFilePath, Global.Utf8WithoutBom);
            }
            // 服务器启动
            if (CommonPath.GamePlatform == "WeGame")
            {
                MessageBox.Show("保存完毕! 请通过WeGame启动,存档文件名为" + CommonPath.GamePlatform + "_" + SaveSlot);
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
                        WorkingDirectory = Path.GetDirectoryName(CommonPath.ServerFilePath) ?? throw new InvalidOperationException(), 
                        // 服务器名字
                        FileName = CommonPath.ServerFilePath,
                        Arguments = "-console -cluster DedicatedServer_" + SaveSlot +" -shard Master"
                    }
                };
                masterProcess.Start();
                // 打开服务器(洞穴)
                if (EditWorldIsCaveSelectBox.Text == "开启")
                {
                    var caveProcess = new Process
                    {
                        StartInfo =
                        {
                            UseShellExecute = false,
                            // 目录,这个必须设置
                            WorkingDirectory = Path.GetDirectoryName(CommonPath.ServerFilePath) ?? throw new InvalidOperationException(), 
                            // 服务器名字
                            FileName = CommonPath.ServerFilePath,
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
        /// 初始化服务器
        /// </summary>
        /// <param name="whenCreateWorld">是否为创建新世界时调用</param>
        public void InitServer(bool whenCreateWorld = false)
        {
            // 设置游戏平台
            CommonPath.SetGamePlatform();
            // 设置游戏平台SelectBox选择
            GamePlatformSelectBox.TextIndex = CommonPath.GetGamePlatform();
            // 设置SaveSlot面板按钮
            SetSaveSlotRadioButton();
            // 汉化
            _Hanization = JsonHelper.ReadHanization();
            // 控制台
            CreateConsoleClassificationButton();
            #region 仅创建世界时
            // [创建世界]设定文件路径
            if (whenCreateWorld)
            {
                _dediFilePath = new DediFilePath(SaveSlot);
            }
            // [创建世界]从modoverrides.lua读取mod设置
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath) && whenCreateWorld)
            {
                SetModSet();
            }
            #endregion
        }

        /// <summary>
        /// 设置SaveSlot面板按钮
        /// </summary>
        private void SetSaveSlotRadioButton()
        {
            // 判断是否存在存档根目录文件夹[不存在则创建]
            if (!Directory.Exists(CommonPath.SaveRootDirPath))
            {
                Directory.CreateDirectory(CommonPath.SaveRootDirPath);
            }
            // 获取服务器路径列表[serverPathList]
            var directoryInfos = new DirectoryInfo(CommonPath.SaveRootDirPath).GetDirectories();
            var serverPathList = (from directoryInfo in directoryInfos where directoryInfo.Name.StartsWith("DedicatedServer_") select directoryInfo.FullName).ToList();
            // 清空SaveSlot面板
            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                ((RadioButton)SaveSlotStackPanel.Children[i]).Content = "新世界";
            }
            if (serverPathList.Count != 0)
            {
                foreach (var serverPath in serverPathList)
                {
                    // 取出序号
                    var num = int.Parse(serverPath.Substring(serverPath.LastIndexOf('_') + 1));
                    // 取出存档名称
                    if (MeasureTextWidth((RadioButton)SaveSlotStackPanel.Children[num], GetClusterName(num)) <= 120)
                        ((RadioButton)SaveSlotStackPanel.Children[num]).Content = GetClusterName(num);
                    else
                    {
                        for (var i = GetClusterName(num).Length; i >= 0; i--)
                        {
                            if (MeasureTextWidth((RadioButton)SaveSlotStackPanel.Children[num], GetClusterName(num).Substring(0, i - 1)) <= 109.17)
                            {
                                ((RadioButton)SaveSlotStackPanel.Children[num]).Content = GetClusterName(num).Substring(0, i - 1) + "...";
                                break;
                            }
                        }
                    }
                }
            }
            // 禁用
            Forbidden(true);
        }

        /// <summary>
        /// 获取存档名[ClusterName]
        /// </summary>
        /// <param name="saveSlot">存档槽</param>
        /// <returns>房间名</returns>
        private static string GetClusterName(int saveSlot)
        {
            var clusterIniPath = CommonPath.SaveRootDirPath + @"\DedicatedServer_" + saveSlot + @"\cluster.ini";
            if (!File.Exists(clusterIniPath))
            {
                return "新世界";
            }
            var iniTool = new IniHelper(clusterIniPath, Global.Utf8WithoutBom);
            var houseName = iniTool.ReadValue("NETWORK", "cluster_name");
            return houseName;
        }

        #region 汉化

        /// <summary>
        /// 汉化[字符串]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="isCave">是否为洞穴</param>
        /// <returns>汉化文本</returns>
        private string Hanization(string key, bool isCave)
        {
            if (!isCave)
            {
                foreach (var item in _Hanization.Hanization.Master.World)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Resources)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Foods)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Animals)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Monsters)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
            }
            else
            {
                foreach (var item in _Hanization.Hanization.Caves.World)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Resources)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Foods)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Animals)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Monsters)
                {
                    if (key == item.Key)
                        return item.KeyHanization;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 汉化[字符串公开枚举数]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="isCave">是否为洞穴</param>
        /// <returns>汉化文本[List]</returns>
        private List<string> HanizationList(string key, bool isCave)
        {
            if (!isCave)
            {
                foreach (var item in _Hanization.Hanization.Master.World)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Resources)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Foods)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Animals)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Master.Monsters)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
            }
            else
            {
                foreach (var item in _Hanization.Hanization.Caves.World)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Resources)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Foods)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Animals)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
                foreach (var item in _Hanization.Hanization.Caves.Monsters)
                {
                    if (key == item.Key)
                        return item.ValueHanization;
                }
            }
            return null;
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
