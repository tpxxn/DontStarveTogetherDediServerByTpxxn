using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace 饥荒开服工具ByTpxxn.Class
{
    public static class Global
    {

        public static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false); // 编码
        /// <summary>
        /// 枚举类型 Message
        /// </summary>
        public enum Message
        {
            保存,
            复活,
            回档,
            重置世界
        }

        /// <summary>
        /// MainPage需要保存在Global里额几个控件对象
        /// </summary>
        public static FontFamily FontFamily { get; set; }
        public static FontWeight FontWeight { get; set; }
        public static Frame DedicatedServerFrame { get; set; }

        /// <summary>
        /// 遍历视觉树
        /// </summary>
        /// <typeparam name="T">泛型T</typeparam>
        /// <param name="results">结果List</param>
        /// <param name="startNode">开始节点</param>
        public static void FindChildren<T>(List<T> results, DependencyObject startNode) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(startNode);
            for (var i = 0; i < count; i++)
            {
                var current = VisualTreeHelper.GetChild(startNode, i);
                if (current.GetType() == typeof(T) || current.GetType().GetTypeInfo().IsSubclassOf(typeof(T)) || current.GetType().IsInstanceOfType(typeof(T)))
                {
                    var asType = (T)current;
                    results.Add(asType);
                }
                FindChildren(results, current);
            }
        }

        /// <summary>
        /// 工程名 [饥荒开服工具ByTpxxn]
        /// </summary>
        public static string ProjectName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 控件可视性设置
        /// </summary>
        /// <param name="visibility">visibility</param>
        /// <param name="obj">控件Name</param>
        public static void UiElementVisibility(Visibility visibility, params UIElement[] obj)
        {
            foreach (var uiElement in obj)
            {
                uiElement.Visibility = visibility;
            }
        }

        /// <summary>
        /// 从工程文件获取流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Stream GetStreamFromProjectFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(ProjectName + "." + path);
        }
    }
}
