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
    /// 2016.11.28 重新写的mod类,代表每单个mod，注意两点：  
    /// 客户端不能显示
    /// 另外看看地窖这个mod是不是有毒，还会总出错吗
    /// </summary>
    internal class Mod
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
            var modInfoLuaTable = LuaConfig.ReadLua(modinfoLuaPath, Encoding.UTF8, false);
            // 读取图片
            try
            {
                var filename = new DirectoryInfo(Path.GetFileName(modinfoLuaPath) ?? throw new InvalidOperationException());
                PicturePath = modInfoLuaTable["icon"]?.ToString() ?? "";
                PicturePath = modinfoLuaPath.Replace(filename.ToString(), "") + PicturePath;
                Picture = new TEXTool().OpenFile(PicturePath);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
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
            #region mod的设置
            if (ModType != ModType.Client)
            {
                // mod的设置 configuration_options
                ConfigurationOptions = new Dictionary<string, ModSetting>();
                // 如果没有设置。返回
                if (modInfoLuaTable["configuration_options"] == null) { return; }
                var configurationOptionsLuaTable = (LuaTable)modInfoLuaTable["configuration_options"];
                //    private Dictionary<string, ModSetting> configuration_options;
                // lua下标从1开始
                for (var i = 1; i <= configurationOptionsLuaTable.Length; i++)
                {
                    // 获取name的值，如果name值为空，干脆不储存，直接到下一个循环,mod中经常会有这种空的东西，不知道是作者故意的还是什么
                    var nameLuaTable = ((LuaTable)configurationOptionsLuaTable[i])["name"];
                    if (nameLuaTable == null || nameLuaTable.ToString().Trim() == "")
                    {
                        continue;
                    }
                    var modName = nameLuaTable.ToString();
                    // Label的值
                    string modLabel;
                    var label = ((LuaTable)configurationOptionsLuaTable[i])["label"];

                    if (label == null || label.ToString().Trim() == "")
                    {
                        modLabel = "";
                    }
                    else
                    {
                        modLabel = label.ToString();
                    }
                    // Hover的值
                    string modHover;
                    var hover = ((LuaTable)configurationOptionsLuaTable[i])["hover"];
                    if (hover == null || hover.ToString().Trim() == "")
                    {
                        modHover = "";
                    }
                    else
                    {
                        modHover = hover.ToString();
                    }
                    // Default的值
                    string modDefault;
                    string modCurrent;
                    var defaultValue = ((LuaTable)configurationOptionsLuaTable[i])["default"];
                    if (defaultValue == null || defaultValue.ToString().Trim() == "")
                    {
                        modDefault = "";
                        modCurrent = "";
                    }
                    else
                    {
                        modDefault = defaultValue.ToString();
                        modCurrent = defaultValue.ToString();
                    }
                    // Options,每个设置的选项
                    var optionsList = new List<Option>();
                    var options = ((LuaTable)configurationOptionsLuaTable[i])["options"];
                    if (options == null)
                    {
                        optionsList = null;
                    }
                    else
                    {
                        var optionsLuaTable = (LuaTable)options;
                        // lua从1开始
                        for (var j = 1; j <= optionsLuaTable.Length; j++)
                        {
                            var option = new Option
                            {
                                Description = ((LuaTable)optionsLuaTable[j])["description"].ToString(),
                                Data = ((LuaTable)optionsLuaTable[j])["data"].ToString()
                            };
                            // 标记，这里没有判断description是否为空，绝大多数都不会出错的，除非作者瞎写。
                            // 其实这个data值是有数据类型的，bool,int，string.但是这里全部都是string了，在保存到文件的时候要判断类型保存
                            optionsList.Add(option);
                        }
                    }
                    // 判断default是否存在于data中，有的作者瞎写。。 只能判断下
                    var isDefaultIndata = false;
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var option in optionsList)
                    {
                        if (modDefault == option.Data)
                        {
                            isDefaultIndata = true;
                        }
                    }
                    // 标记（listOptions[0]没有判断是否为空） 如果不存在，赋值第一个data的值
                    if (!isDefaultIndata)
                    {
                        modDefault = optionsList[0].Data;
                        modCurrent = optionsList[0].Data;
                    }
                    // 赋值到mod设置中
                    var modSetting = new ModSetting
                    {
                        Name = modName,
                        Label = modLabel,
                        Hover = modHover,
                        Current = modCurrent,
                        Default = modDefault,
                        Options = optionsList
                    };
                    // 添加到总的configuration_options
                    ConfigurationOptions[modName] = modSetting;
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
            foreach (var item in iDictionary)
            {
                //  如果不存在，下一循环
                if (!ConfigurationOptions.ContainsKey(item.Key))
                {
                    continue;
                }
                // 赋值到当前值,【到这里，用当前值覆盖了default，如果没有被覆盖的就是默认值】
                if (item.Value != null)
                {
                    ConfigurationOptions[item.Key].Current = item.Value.ToString();
                }
            }
        }
    }
}
