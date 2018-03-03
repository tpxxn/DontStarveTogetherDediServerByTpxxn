using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal static class StringProcess
    {
        /// <summary>
        /// 删除重复数据
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        public static string[] StringDelRepeatData(string[] stringArray)
        {
            var OrderlyStringArray = stringArray.GroupBy(p => p).Select(p => p.Key).ToArray();
            if (OrderlyStringArray.Length != 1) return OrderlyStringArray;
            var tempStringList = new List<string>
            {
                OrderlyStringArray[0],
                ""
            };
            OrderlyStringArray = tempStringList.ToArray();
            return OrderlyStringArray;
        }
        
        /// <summary>
        /// 返回Json文本
        /// </summary>
        /// <param name="fileName">文件名[相对于Json文件夹]</param>
        /// <returns>string类型文本</returns>
        public static string GetJsonString(string fileName)
        {
            var src2 = Application.GetResourceStream(new Uri("/饥荒开服工具ByTpxxn;component/Json/" + fileName, UriKind.Relative))?.Stream;
            var str = new StreamReader(src2 ?? throw new InvalidOperationException(), Encoding.UTF8).ReadToEnd();
            return str;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path">长字符串</param>
        /// <returns>资源文件路径</returns>
        public static string GetFileName(string path)
        {
            path = path.Substring(path.LastIndexOf('/') + 1, path.Length - path.LastIndexOf('/') - 5);
            return path;
        }

        /// <summary>
        /// 检查控制台数字文本框
        /// </summary>
        /// <param name="textbox">文本框对象</param>
        public static void ConsoleNumTextCheck(TextBox textbox)
        {
            try
            {
                if (!Regex.IsMatch(textbox.Text, "^\\d*\\.?\\d*$") && textbox.Text != "")
                {
                    int pos = textbox.SelectionStart - 1;
                    textbox.Text = textbox.Text.Remove(pos, 1);
                    textbox.SelectionStart = pos;
                }
                if (textbox.Text != "")
                {
                    if (int.Parse(textbox.Text) > 1000)
                    {
                        textbox.Text = "1000";
                    }
                }
            }
            catch (Exception)
            {
                textbox.Text = "1";
            }
        }

        /// <summary>
        /// 检查QQ文本框
        /// </summary>
        /// <param name="textbox">文本框对象</param>
        public static void QqNumTextCheck(TextBox textbox)
        {
            if (!Regex.IsMatch(textbox.Text, "^\\d*\\.?\\d*$") && textbox.Text != "")
            {
                int pos = textbox.SelectionStart - 1;
                textbox.Text = textbox.Text.Remove(pos, 1);
                textbox.SelectionStart = pos;
            }
        }
    }
}
