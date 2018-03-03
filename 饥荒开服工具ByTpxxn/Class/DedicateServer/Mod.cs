using System;
using Neo.IronLua;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using TEXRead;
using 饥荒开服工具ByTpxxn.Class.Tools;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    /// <summary>
    /// mod的类型
    /// </summary>
    public enum ModType
    {
        Client = 0,
        Server = 1,
        AllClient = 2
    }

    /// <summary>
    /// mod类,代表每单个mod<para/>  
    /// 客户端不能显示<para/>
    /// </summary>
    // TODO 看看地窖这个mod是不是有毒，还会总出错吗
    public class Mod
    {
        #region mod属性
        /// <summary>
        /// mod的文件夹名
        /// </summary>
        public string ModDirName { get; set; }

        /// <summary>
        /// modinfo.lua的全路径
        /// </summary>
        public string ModinfoLuaPath { get; set; }


        /// <summary>
        /// mod图路径
        /// </summary>
        public string PicturePath { get; set; }

        /// <summary>
        /// mod图[Picture]
        /// </summary>
        public Bitmap Picture { get; set; }

        /// <summary>
        /// mod名[Name]
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// mod描述[Description]
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// mod作者[Author]
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// mod版本[Version]
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// mod的类型
        /// </summary>
        public ModType ModType { get; set; }

        /// <summary>
        /// mod设置[configuration_options]
        /// </summary>
        internal Dictionary<string, ModSetting> ConfigurationOptions { get; set; }

        /// <summary>
        /// Mod是否启用
        /// </summary>
        public bool Enabled { get; set; }
        #endregion

        /// <summary>
        /// Mod构造事件
        /// </summary> 
        /// <param name="modinfoLuaPath">mod的modinfo.lua文件路径</param>
        public Mod(string modinfoLuaPath)
        {
            #region Mod除了设置的各种信息
            // 路径
            ModinfoLuaPath = modinfoLuaPath;
            //文件夹名字
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(modinfoLuaPath) ?? throw new InvalidOperationException());
            ModDirName = directoryInfo.Name;
            // 读取modinfo文件
            var modInfoLuaTable = LuaHelper.ReadLua(modinfoLuaPath, Encoding.UTF8, false);
            // 读取图片
            try
            {
                var filename = new DirectoryInfo(Path.GetFileName(modinfoLuaPath) ?? throw new InvalidOperationException());
                PicturePath = modInfoLuaTable["icon"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(PicturePath))
                {
                    PicturePath = modinfoLuaPath.Replace(filename.ToString(), "") + PicturePath;
                    Debug.WriteLine("图片路径：" + PicturePath);
                    Picture = new TEXTool().OpenFile(PicturePath);
                    Debug.WriteLine("图片：" + Picture);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("读取图片错误，错误消息：" + e.Message);
            }
            // 读取其他元信息
            Name = modInfoLuaTable["name"]?.ToString() ?? "";
            Description = modInfoLuaTable["description"]?.ToString() ?? "";
            Author = modInfoLuaTable["author"]?.ToString() ?? "";
            Version = modInfoLuaTable["version"]?.ToString() ?? "";
            // mod类型[客户端|服务端|所有人]
            if (modInfoLuaTable["client_only_mod"] == null || modInfoLuaTable["client_only_mod"].ToString().Trim().ToLower() == "false")
            {
                if (modInfoLuaTable["all_clients_require_mod"] == null)
                {
                    ModType = ModType.AllClient;
                }
                else
                {
                    ModType = modInfoLuaTable["all_clients_require_mod"].ToString().Trim().ToLower() == "true" ? ModType.AllClient : ModType.Server;
                }
            }
            else
            {
                ModType = ModType.Client;
            }
            #endregion
            #region mod的设置信息
            if (ModType != ModType.Client)
            {
                // mod的设置 configuration_options
                ConfigurationOptions = new Dictionary<string, ModSetting>();
                // 如果没有设置，返回
                if (modInfoLuaTable["configuration_options"] == null) { return; }
                var configurationOptionsLuaTable = (LuaTable)modInfoLuaTable["configuration_options"];
                // lua下标从1开始
                for (var i = 1; i <= configurationOptionsLuaTable.Length; i++)
                {
                    // 获取name的值[name设置的是唯一标志符]，如果name值为空，干脆不储存，直接到下一个循环
                    var nameLuaTable = ((LuaTable)configurationOptionsLuaTable[i])["name"];
                    if (string.IsNullOrEmpty(nameLuaTable?.ToString().Trim()))
                        continue;
                    var modSettingName = nameLuaTable.ToString();
                    // Label的值
                    var label = ((LuaTable)configurationOptionsLuaTable[i])["label"];
                    var modSettingLabel = string.IsNullOrEmpty(label?.ToString().Trim()) ? "" : label.ToString();
                    // Hover的值
                    var hover = ((LuaTable)configurationOptionsLuaTable[i])["hover"];
                    var modSettingHover = string.IsNullOrEmpty(hover?.ToString().Trim()) ? "" : hover.ToString();
                    // Options,每个设置的选项
                    List<Option> modSettingOptionList = null;
                    var options = ((LuaTable)configurationOptionsLuaTable[i])["options"];
                    if (options != null)
                    {
                        modSettingOptionList = new List<Option>();
                        var optionsLuaTable = (LuaTable)options;
                        // lua从1开始
                        for (var j = 1; j <= optionsLuaTable.Length; j++)
                        {
                            var optionsHover = ((LuaTable)optionsLuaTable[j])["hover"];
                            var option = new Option
                            {
                                Description= ((LuaTable)optionsLuaTable[j])["description"].ToString(),
                                Data = ((LuaTable)optionsLuaTable[j])["data"].ToString()
                            };
                            if (!string.IsNullOrEmpty(optionsHover?.ToString().Trim()))
                                option.Hover = optionsHover.ToString();
                            // 其实这个data值是有数据类型的，bool,int，string.但是这里全部都是string了，在保存到文件的时候要判断类型保存
                            modSettingOptionList.Add(option);
                        }
                    }
                    // Default的值
                    var modSettingDefault = "";
                    var modSettingCurrent = "";
                    var defaultValue = ((LuaTable)configurationOptionsLuaTable[i])["default"];
                    if (!string.IsNullOrEmpty(defaultValue?.ToString().Trim()))
                    {
                        modSettingDefault = defaultValue.ToString();
                        modSettingCurrent = defaultValue.ToString();
                    }
                    // 判断default是否存在于data中，有的作者瞎写。。 只能判断下
                    var isDefaultInData = false;
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var option in modSettingOptionList)
                    {
                        if (modSettingDefault == option.Data)
                        {
                            isDefaultInData = true;
                        }
                    }
                    // 标记（listOptions[0]没有判断是否为空） 如果不存在，赋值第一个data的值
                    if (!isDefaultInData)
                    {
                        modSettingDefault = modSettingOptionList[0].Data;
                        modSettingCurrent = modSettingOptionList[0].Data;
                    }
                    // 赋值到mod设置中
                    var modSetting = new ModSetting
                    {
                        Name = modSettingName,
                        Label = modSettingLabel,
                        Hover = modSettingHover,
                        Options = modSettingOptionList,
                        Default = modSettingDefault,
                        Current = modSettingCurrent
                    };
                    // 添加到总的 configuration_options
                    ConfigurationOptions[modSettingName] = modSetting;
                }
            }
            #endregion
            //#region 读取modoverrides，赋值到current值中，用current覆盖default
            //ReadModoverrides(thisModConfig);
            //#endregion
        }

        /// <summary>
        /// 读取modoverrides，赋值到current值中，用current覆盖default
        /// </summary>
        /// <param name="modConfig"></param>
        public void ReadModoverrides(LuaTable modConfig)
        {
            // 如果为空，说明没有开启此mod，返回
            if (modConfig == null) return;
            Enabled = (bool)modConfig["enabled"];
            // 储存enabled
            //// enable 为false，说明没有开启mod，返回
            //if (Enabled == false) { return; }
            var modConfigurationOptions = modConfig["configuration_options"];
            // 如果没有设置配置，还是返回
            if (modConfigurationOptions == null) return;
            // 格式转换
            var iDictionary = ((LuaTable)modConfigurationOptions).Members;
            foreach (var keyValuePair in iDictionary)
            {
                //  如果不存在，下一循环
                if (!ConfigurationOptions.ContainsKey(keyValuePair.Key))
                {
                    continue;
                }
                // 赋值到当前值,［到这里，用当前值覆盖了default，如果没有被覆盖的就是默认值］
                if (keyValuePair.Value != null)
                {
                    ConfigurationOptions[keyValuePair.Key].Current = keyValuePair.Value.ToString();
                }
            }
        }
    }
}
