using Neo.IronLua;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using 饥荒开服工具ByTpxxn.Class.Tools;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    /// <summary>
    /// 所有mod集合到一起
    /// </summary>
    class Mods
    {

        #region 字段和属性
        /// <summary>
        /// Modoverride文件路径
        /// </summary>
        public string ModoverrideFilePath { get; set; }

        /// <summary>
        /// Mod列表
        /// </summary>
        internal List<Mod> ModList { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 读取所有的mod，放到ModList中
        /// </summary>
        public Mods()
        {
            ModList = new List<Mod>();
            // 遍历modsPath中每一个文件modinfo.lua文件
            var directoryInfos = new DirectoryInfo(CommonPath.ServerModsDirPath).GetDirectories();
            // TODO：这里要保证mods文件夹下全部都是mod的文件夹，不能有其他的文件夹，不然后面可能会出错
            foreach (var directoryInfoInDirectoryInfos in directoryInfos)
            {
                try
                {
                    // modinfo的路径
                    var modinfoPath = directoryInfoInDirectoryInfos.FullName + @"\modinfo.lua";
                    // 创建mod
                    var mod = new Mod(modinfoPath);
                    // [如果不是客户端mod]添加
                    if (mod.ModType != ModType.Client)
                        ModList.Add(mod);
                    else
                        // [若为客户端mod]删除
                        Directory.Delete(directoryInfoInDirectoryInfos.FullName, true);
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            Debug.WriteLine("读取mod完成！");
        }

        public void ReadModsOverrides(string modoverridesFilePath)
        {
            ModoverrideFilePath = modoverridesFilePath;
            var serverluaTable = LuaHelper.ReadLua(modoverridesFilePath, Encoding.UTF8, true);
            // 遍历modsPath中每一个文件modinfo.lua文件
            var directoryInfos = new DirectoryInfo(CommonPath.ServerModsDirPath).GetDirectories();
            // TODO：这里要保证mods文件夹下全部都是mod的文件夹，不能有其他的文件夹，不然后面可能会出错
            for (var i = 0; i < directoryInfos.Length; i++)
            {
                if (i >= ModList.Count)
                    break;
                // 这个mod的配置luaTable，可以为空，后面有判断
                var luaTable = serverluaTable[directoryInfos[i].Name] == null ? null : (LuaTable)serverluaTable[directoryInfos[i].Name];
                // 读取modoverrides，赋值到current值中，用current覆盖default
                ModList[i].ReadModoverrides(luaTable);
            }
        }

        #endregion

        #region ［保存到文件］把listmods保存到文件,保存的时候注意格式

        /// <summary>
        /// 把listmods保存到文件,保存的时候注意格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">编码</param>
        public void SaveListmodsToFile(string filePath, Encoding encoding)
        {
            // 开始拼接字符串
            var modSettingStringBuilder = new StringBuilder();
            // 循环获取
            foreach (var mod in ModList)
            {
                // mod的文件夹名字
                var modDirName = mod.ModDirName;
                // mod是否开启
                var modEnabled = mod.Enabled;
                // 如果mod没有开启，则不拼接不写入文件。
                if (!modEnabled) continue;
                // mod的设置拼接,存在设置才会拼接,if判断
                var modSetting = "";
                if (mod.ConfigurationOptions.Count != 0)
                {

                    var configurationOptionsStringBuilder = new StringBuilder();
                    var configurationOptionsDictionary = mod.ConfigurationOptions;
                    foreach (var configurationOption in configurationOptionsDictionary)
                    {
                        configurationOptionsStringBuilder.AppendFormat(Indent(3) + "{0} = {1},\r\n", KeyType(configurationOption.Key), ValueType(configurationOption.Value.Current));

                    }
                    modSetting = Indent(2) + "configuration_options={{\r\n" + configurationOptionsStringBuilder + Indent(2) + "}},\r\n";
                }
                // 大的拼接
                if(string.IsNullOrEmpty(modSetting))
                    modSettingStringBuilder.AppendFormat(Indent(1) + "[\"{0}\"] = {{ enabled = {1} }},\r\n", modDirName, "true");
                else
                    modSettingStringBuilder.AppendFormat(Indent(1) + "[\"{0}\"] = {{\r\n" + modSetting + Indent(2) + "enabled = {1}\r\n" + Indent(1) + " }},\r\n", modDirName, "true");
            }
            // 拼接成最后的文件，创建，保存： modoverrides.lua
            File.WriteAllText(filePath, "return {\r\n" + modSettingStringBuilder + "\r\n}", encoding);
        }

        /// <summary>
        /// 缩进
        /// </summary>
        /// <param name="indent">缩进数</param>
        /// <param name="indentSpace">单个缩进的空格数</param>
        /// <returns>缩进空格字符串</returns>
        private static string Indent(int indent, int indentSpace = 2)
        {
            var indentString = "";
            for (var i = 0; i < indent; i++)
            {
                for (var j = 0; j < indentSpace; j++)
                {
                    indentString += " ";
                }
            }
            return indentString;
        }

        /// <summary>
        /// 按照格式来保存
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns>转换后的键字符串</returns>
        private static string KeyType(string keyString)
        {
            //如果包含非英文
            if (Regex.IsMatch(keyString, "[^a-zA-Z0-9]") || keyString == "")
            {
                return "[\"" + keyString + "\"]";
            }
            return keyString;
        }

        /// <summary>
        /// 按照格式来保存
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns>转换后的值字符串</returns>
        private static string ValueType(string valueString)
        {
            // 判断是不是bool类型
            if (bool.TryParse(valueString, out _))
            {
                return valueString.ToLower();
            }
            // 判断是不是数字类型
            if (double.TryParse(valueString, out _))
            {
                return valueString;
            }
            return "\"" + valueString + "\"";

        }
        #endregion
    }
}
