using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    /// <summary>
    /// ini文件读写类
    /// </summary>
    public class IniHelper
    {
        #region 字段

        /// <summary>
        /// ini对象[格式：字典]
        /// </summary>
        public readonly Dictionary<string, Dictionary<string, string>> iniObjectDictionary;

        /// <summary>
        /// ini文件文本内容
        /// </summary>
        public readonly string[] IniFileContext;

        /// <summary>
        /// ini文件路径
        /// </summary>
        public readonly string IniFilePath;

        #endregion

        /// <summary>
        /// IniHelper
        /// </summary>
        /// <param name="path">ini文件路径</param>
        /// <param name="encoding">编码格式</param>
        public IniHelper(string path, Encoding encoding)
        {
            IniFilePath = path;
            if (File.Exists(path))
            {
                IniFileContext = File.ReadAllLines(path, encoding);
            }
            iniObjectDictionary = new Dictionary<string, Dictionary<string, string>>();
            var section = string.Empty;
            foreach (var oneLine in IniFileContext)
            {
                // 获取单行文本[去除空格]
                var line = oneLine.Trim();
                // [节]如果以[] 开头结尾,其实用正则更好
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // 截取[] 中间
                    section = line.Substring(1, line.Length - 2);
                    // 添加到d
                    if (section != string.Empty)
                    {
                        if (!iniObjectDictionary.ContainsKey(section))
                            iniObjectDictionary.Add(section, new Dictionary<string, string>());
                    }
                }
                // [参数]
                if (line.Contains("="))
                {
                    var parametersArray = line.Split('=');
                    iniObjectDictionary[section].Add(parametersArray[0].Trim(), parametersArray[1].Trim());
                }
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">参数[键]</param>
        /// <returns>参数[值]</returns>
        public string ReadValue(string section, string key)
        {
            if (iniObjectDictionary == null)
                return "";
            if(!iniObjectDictionary.ContainsKey(section))
                return "";
            return !iniObjectDictionary[section].ContainsKey(key) ? "" : iniObjectDictionary[section][key];
        }

        /// <summary>
        /// 写入到文件
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">参数[键]</param>
        /// <param name="value">参数[值]</param>
        /// <param name="encoding">编码格式</param>
        public void WriteValue(string section, string key, string value, Encoding encoding)
        {
            iniObjectDictionary[section][key] = value;
            File.WriteAllLines(IniFilePath, GetListStr(), encoding);
        }

        /// <summary>
        /// 从Dictionary序列化为ini文本
        /// </summary>
        /// <returns></returns>
        private List<string> GetListStr()
        {
            var iniStringList = new List<string>();
            foreach (var sectionKeyValuePair in iniObjectDictionary)
            {
                iniStringList.Add("[" + sectionKeyValuePair.Key + "]");
                iniStringList.AddRange(sectionKeyValuePair.Value.Select(parameterKeyValuePair => parameterKeyValuePair.Key + " = " + parameterKeyValuePair.Value));
                iniStringList.Add("");
                iniStringList.Add("");
            }
            return iniStringList;
        }
    }
}
