using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 饥荒开服工具ByTpxxn.Class
{
    /// <summary>
    /// 用于读写软件配置
    /// </summary>
    internal static class IniFileIo
    {
        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        public static string IniFileReadString(string fileName, string section, string key)
        {
            // ini文件名
            var iniFileName = fileName + ".ini";
            // 创建新的ini文件解析实例
            var fileIniDataParser = new FileIniDataParser();
            // 判断文件是否存在
            if (!File.Exists(iniFileName))
            {
                File.Create(iniFileName).Close();
            }
            // 解析ini文件
            var iniData = fileIniDataParser.ReadFile(iniFileName);
            // 获取值
            if (!iniData.Sections.ContainsSection(section))
                return "";
            return iniData.Sections.GetSectionData(section).Keys.ContainsKey(key) ? iniData[section][key] : "";
        }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        public static double IniFileReadDouble(string fileName, string section, string key)
        {
            // ini文件名
            var iniFileName = fileName + ".ini";
            // 创建新的ini文件解析实例
            var fileIniDataParser = new FileIniDataParser();
            // 判断文件是否存在
            if (!File.Exists(iniFileName))
            {
                // 若文件不存在抛出异常
                File.Create(iniFileName);
            }
            // 解析ini文件
            var iniData = fileIniDataParser.ReadFile(iniFileName);
            // 获取值
            if (!iniData.Sections.ContainsSection(section))
                return 0;
            return iniData.Sections.GetSectionData(section).Keys.ContainsKey(key) ? double.Parse(iniData[section][key]) : 0;
        }

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void IniFileWrite(string fileName, string section, string key, string value)
        {
            // ini文件名
            var iniFileName = fileName + ".ini";
            // 创建新的ini文件解析实例
            var fileIniDataParser = new FileIniDataParser();
            // ini数据对象
            var iniData = new IniData();
            // 判断文件是否存在
            if (File.Exists(iniFileName))
            {
                iniData = fileIniDataParser.ReadFile(iniFileName);
            }
            // 判断节点是否存在，不存在则添加节点
            if(!iniData.Sections.ContainsSection(section))
                iniData.Sections.AddSection(section);
            // 判断键是否存在，不存在则添加键，存在则修改键
            if (!iniData.Sections.GetSectionData(section).Keys.ContainsKey(key))
                iniData.Sections.GetSectionData(section).Keys.AddKey(key, value);
            else
                iniData[section][key] = value;
            // 写入ini文件
            fileIniDataParser.WriteFile(iniFileName, iniData);
        }

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="keyValuePair">键值对</param>
        public static void IniFileWrite(string fileName, string section, Dictionary<string ,string> keyValuePair)
        {
            // ini文件名
            var iniFileName = fileName + ".ini";
            // 创建新的ini文件解析实例
            var fileIniDataParser = new FileIniDataParser();
            // ini数据对象
            var iniData = new IniData();
            // 判断文件是否存在
            if (File.Exists(iniFileName))
            {
                iniData = fileIniDataParser.ReadFile(iniFileName);
            }
            // 判断节点是否存在，不存在则添加节点
            if (!iniData.Sections.ContainsSection(section))
                iniData.Sections.AddSection(section);
            // 判断键是否存在，不存在则添加键，存在则修改键
            foreach (var keyValue in keyValuePair)
            {
                if (!iniData.Sections.GetSectionData(section).Keys.ContainsKey(keyValue.Key))
                    iniData.Sections.GetSectionData(section).Keys.AddKey(keyValue.Key, keyValue.Value);
                else
                    iniData[section][keyValue.Key] = keyValue.Value;
            }
            // 写入ini文件
            fileIniDataParser.WriteFile(iniFileName, iniData);
        }
    }
}
