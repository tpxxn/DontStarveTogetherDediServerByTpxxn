using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using 饥荒开服工具ByTpxxn.Class.Tools;


namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    /// <summary>
    /// 基本设置类
    /// 添加字段和属性后，记得在构造函数附一个初始值
    /// </summary>
    internal class BaseSet : INotifyPropertyChanged
    {
        #region 字段、属性

        #region 字段

        /// <summary>
        /// FileToProperty标志
        /// </summary>
        private bool _isFileToProperty;

        /// <summary>
        /// cluster.ini路径
        /// </summary>
        private readonly string _clusterIniFilePath;

        #endregion

        #region 属性
        
        /// <summary>
        /// 基本设置-[当前显示的]游戏风格
        /// </summary>
        private string _gameStyle;

        public string GameStyle
        {
            get => _gameStyle;
            set
            {
                _gameStyle = value;
                NotifyPropertyChange("GameStyle");
                if (!_isFileToProperty) { SavePropertyToFile("GameStyle"); }
            }
        }

        /// <summary>
        /// 基本设置-名称
        /// </summary>
        private string _clusterName;

        public string ClusterName
        {
            get => _clusterName;
            set
            {
                _clusterName = value;
                if (!_isFileToProperty) { SavePropertyToFile("ClusterName"); }
                NotifyPropertyChange("ClusterName");
            }
        }

        /// <summary>
        /// 基本设置-描述
        /// </summary>
        private string _describe;

        public string Describe
        {
            get => _describe;
            set
            {
                _describe = value;
                NotifyPropertyChange("Describe");
                if (!_isFileToProperty) { SavePropertyToFile("Describe"); }
            }
        }
        
        /// <summary>
        /// [当前显示的]游戏模式
        /// </summary>
        private int _gameMode;

        public int GameMode
        {
            get => _gameMode;
            set
            {
                _gameMode = value;
                NotifyPropertyChange("GameMode");
                if (!_isFileToProperty) { SavePropertyToFile("GameMode"); }
            }
        }

        /// <summary>
        /// 基本设置-PVP[是|否]
        /// </summary>
        private int _isPvp;

        public int IsPvp
        {
            get => _isPvp;
            set
            {
                _isPvp = value;
                NotifyPropertyChange("IsPVP");
                if (!_isFileToProperty) { SavePropertyToFile("IsPVP"); }
            }
        }

        /// <summary>
        /// 基本设置-玩家
        /// </summary>
        private int _maxPlayers;

        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                _maxPlayers = value;
                NotifyPropertyChange("LimitNumOfPeople");
                if (!_isFileToProperty) { SavePropertyToFile("MaxPlayers"); }
            }
        }

        /// <summary>
        /// 基本设置-密码
        /// </summary>
        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChange("Password");
                if (!_isFileToProperty) { SavePropertyToFile("Password"); }
            }
        }

        /// <summary>
        /// 基本设置-服务器模式[在线|离线]
        /// </summary>
        private int _serverMode;

        public int ServerMode
        {
            get => _serverMode;
            set
            {
                _serverMode = value;
                if (!_isFileToProperty) { SavePropertyToFile("ServerMode"); }
            }
        }

        /// <summary>
        /// 基本设置-无人时暂停
        /// </summary>
        private int _isPause;

        public int IsPause
        {
            get => _isPause;
            set
            {
                _isPause = value;
                NotifyPropertyChange("IsPause");
                if (!_isFileToProperty) { SavePropertyToFile("IsPause"); }
            }
        }

        /// <summary>
        /// 编辑世界-是否开启洞穴
        /// </summary>
        private int _isCave;

        public int IsCave
        {
            get => _isCave;
            set
            {
                _isCave = value;
                NotifyPropertyChange("IsCave");
                if (!_isFileToProperty) { SavePropertyToFile("IsCave"); }
            }
        }

        /// <summary>
        /// 是否开启控制台
        /// </summary>
        private int _isConsole;

        public int IsConsole
        {
            get => _isConsole;
            set
            {
                _isConsole = value;
                NotifyPropertyChange("IsConsole");
                if (!_isFileToProperty) { SavePropertyToFile("IsConsole"); }
            }
        }

        #endregion

        #endregion

        #region 构造函数 

        /// <summary>
        /// BaseSet构造函数
        /// </summary>
        /// <param name="clusterIniFilePath">cluster.ini路径</param>
        public BaseSet(string clusterIniFilePath)
        {
            if (File.Exists(clusterIniFilePath))
            {
                // 设定cluster.ini文件路径
                _clusterIniFilePath = clusterIniFilePath;
                // 从文件读，给字段赋值
                FileToProperty(clusterIniFilePath);
            }
            else
            {
                Debug.WriteLine("cluster.ini文件不存在");
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 读取
        /// </summary>
        private void FileToProperty(string clusterIniPath)
        {
            // 改变记号（这个记号可能以后保存的时候会有用）
            _isFileToProperty = true;

            // 这里没有判断文件是否存在，在外面判断了，以后再看用不用修改

            // 读取基本设置
            var clusterIniFile = new IniHelper(clusterIniPath, Global.Utf8WithoutBom);

            // 读取游戏风格
            var gameStyle = clusterIniFile.ReadValue("NETWORK", "cluster_intention");
            if (gameStyle == "cooperative") { GameStyle = "合作"; }
            if (gameStyle == "social") { GameStyle = "交际"; }
            if (gameStyle == "competitive") { GameStyle = "竞争"; }
            if (gameStyle == "madness") { GameStyle = "疯狂"; }

            // 读取房间名称
            ClusterName = clusterIniFile.ReadValue("NETWORK", "cluster_name");

            // 读取描述
            Describe = clusterIniFile.ReadValue("NETWORK", "cluster_description");

            // 读取游戏模式
            var gameMode = clusterIniFile.ReadValue("GAMEPLAY", "game_mode");
            if (gameMode == "endless") { GameMode = 0; }
            if (gameMode == "survival") { GameMode = 1; }
            if (gameMode == "wilderness") { GameMode = 2; }

            // 读取PVP[标记：这里没有变成小写]
            var pvp = clusterIniFile.ReadValue("GAMEPLAY", "pvp");
            if (pvp == "false") { IsPvp = 0; }
            if (pvp == "true") { IsPvp = 1; }

            // 读取人数限制
            MaxPlayers = int.Parse(clusterIniFile.ReadValue("GAMEPLAY", "max_players")) - 1;

            // 读取密码
            Password = clusterIniFile.ReadValue("NETWORK", "cluster_password");

            // 读取服务器模式 offline_cluster=true
            var offlineCluster = clusterIniFile.ReadValue("NETWORK", "offline_cluster");
            if (offlineCluster == "true") { ServerMode = 0; }
            if (offlineCluster == "false") { ServerMode = 1; }

            // 读取无人时暂停[标记：这里没有变成小写]
            var pauseWhenEmpty = clusterIniFile.ReadValue("GAMEPLAY", "pause_when_empty");
            if (pauseWhenEmpty == "false") { IsPause = 0; }
            if (pauseWhenEmpty == "true") { IsPause = 1; }

            // 读取是否开启洞穴[标记：这里没有变成小写]
            var cave = clusterIniFile.ReadValue("SHARD", "shard_enabled");
            if (cave == "false") { IsCave = 0; }
            if (cave == "true") { IsCave = 1; }

            // 读取是否启用控制台[标记：这里没有变成小写]
            var console = clusterIniFile.ReadValue("MISC", "console_enabled");
            if (console == "false") { IsConsole = 0; }
            if (console == "true") { IsConsole = 1; }

            _isFileToProperty = false;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="propertyName"></param>
        public void SavePropertyToFile(string propertyName)
        {
            // 保存
            if (_isFileToProperty == false)
            {
                var clusterIniFile = new IniHelper(_clusterIniFilePath, Global.Utf8WithoutBom);
                switch (propertyName)
                {
                    case "GameStyle":
                        if (GameStyle == "合作") { clusterIniFile.Write("NETWORK", "cluster_intention", "cooperative", Global.Utf8WithoutBom); }
                        if (GameStyle == "交际") { clusterIniFile.Write("NETWORK", "cluster_intention", "social", Global.Utf8WithoutBom); }
                        if (GameStyle == "竞争") { clusterIniFile.Write("NETWORK", "cluster_intention", "competitive", Global.Utf8WithoutBom); }
                        if (GameStyle == "疯狂") { clusterIniFile.Write("NETWORK", "cluster_intention", "madness", Global.Utf8WithoutBom); }
                        break;
                    case "ClusterName":
                        clusterIniFile.Write("NETWORK", "cluster_name", ClusterName, Global.Utf8WithoutBom);
                        break;
                    case "Describe":
                        clusterIniFile.Write("NETWORK", "cluster_description", Describe, Global.Utf8WithoutBom);
                        break; 
                    case "GameMode":
                        if (GameMode == 0) { clusterIniFile.Write("GAMEPLAY", "game_mode", "endless", Global.Utf8WithoutBom); }
                        if (GameMode == 1) { clusterIniFile.Write("GAMEPLAY", "game_mode", "survival", Global.Utf8WithoutBom); }
                        if (GameMode == 2) { clusterIniFile.Write("GAMEPLAY", "game_mode", "wilderness", Global.Utf8WithoutBom); }
                        break;
                    case "IsPVP":
                        clusterIniFile.Write("GAMEPLAY", "pvp", IsPvp == 0 ? "false" : "true", Global.Utf8WithoutBom);
                        break;
                    case "MaxPlayers":
                        clusterIniFile.Write("GAMEPLAY", "max_players", (MaxPlayers + 1).ToString(), Global.Utf8WithoutBom);
                        break;
                    case "Password":
                        clusterIniFile.Write("NETWORK", "cluster_password", Password, Global.Utf8WithoutBom);
                        break;
                    case "ServerMode":
                        if (ServerMode == 0) { clusterIniFile.Write("NETWORK", "offline_cluster", "false", Global.Utf8WithoutBom); }
                        if (ServerMode == 1) { clusterIniFile.Write("NETWORK", "offline_cluster", "true", Global.Utf8WithoutBom); }
                        break;
                    case "IsPause":
                        clusterIniFile.Write("GAMEPLAY", "pause_when_empty", IsPause == 0 ? "false" : "true", Global.Utf8WithoutBom);
                        break;
                    case "IsCave":
                        clusterIniFile.Write("SHARD", "shard_enabled", IsCave == 0 ? "false" : "true", Global.Utf8WithoutBom);
                        break;
                    case "IsConsole":
                        clusterIniFile.Write("MISC", "console_enabled", IsConsole == 0 ? "false" : "true", Global.Utf8WithoutBom);
                        break;
                }
            }
        }
        #endregion

        #region 委托

        /// <summary>
        /// 属性变更委托
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更
        /// </summary>
        /// <param name="propertyName">属性名</param>
        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
