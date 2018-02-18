namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal class PathFile
    {
        #region 字段、属性

        /// <summary>
        /// Server路径
        /// </summary>
        private string _serverDirPath;

        public string ServerDirPath
        {
            get => _serverDirPath;
            set
            {
                _serverDirPath = value;
                if (!string.IsNullOrEmpty(_serverDirPath))
                {
                    OverworldConfigFilePath = _serverDirPath + "\\Master\\leveldataoverride.lua";
                    CaveConfigFilePath = _serverDirPath + "\\Caves\\leveldataoverride.lua";
                    ModConfigOverworldFilePath = _serverDirPath + "\\Master\\modoverrides.lua";
                    ModConfigCaveFilePath = _serverDirPath + "\\Caves\\modoverrides.lua";
                }
            }
        }

        /// <summary>
        /// 地上世界-配置-路径
        /// </summary>
        public string OverworldConfigFilePath { get; set; }

        /// <summary>
        /// 地下世界-配置-路径
        /// </summary>
        public string CaveConfigFilePath { get; set; }

        /// <summary>
        /// 地上世界-mod-设置
        /// </summary>
        public string ModConfigOverworldFilePath { get; set; }

        /// <summary>
        /// 地下世界-mod-设置
        /// </summary>
        public string ModConfigCaveFilePath { get; set; }
        
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="saveSlot">存档槽</param>
        public PathFile(int saveSlot)
        {
            SetAllPath(PathCommon.GamePlatform, saveSlot);
        }

        /// <summary>
        /// 设置所有路径
        /// </summary>
        /// <param name="gamePlatform">游戏平台</param>
        /// <param name="saveSlot">存档槽</param>
        public void SetAllPath(string gamePlatform, int saveSlot = 0)
        {
            if (!string.IsNullOrEmpty(PathCommon.SaveRootDirPath))
            {
                ServerDirPath = PathCommon.SaveRootDirPath + @"\DedicatedServer_" + saveSlot;
            }
        }

    }
}
