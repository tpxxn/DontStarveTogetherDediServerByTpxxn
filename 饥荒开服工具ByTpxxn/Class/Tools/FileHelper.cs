using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal class FileHelper
    {
        /// <summary>
        /// 读取资源文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadResources(string path)
        {
            var utf8Encoding = new UTF8Encoding(false);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(Global.ProjectName + "." + path);
            var streamReader = new StreamReader(stream, Encoding.UTF8);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// 拷贝文件夹,会覆盖!!
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        public static void CopyDirectory(string fromPath, string toPath)
        {
            //如果源文件夹不存在，则创建
            if (!Directory.Exists(fromPath))
            {
                Directory.CreateDirectory(fromPath);
            }
            //取得要拷贝的文件夹名
            var strFolderName = fromPath.Substring(fromPath.LastIndexOf("\\", StringComparison.Ordinal) +
              1, fromPath.Length - fromPath.LastIndexOf("\\", StringComparison.Ordinal) - 1);
            //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
            if (!Directory.Exists(toPath + "\\" + strFolderName))
            {
                Directory.CreateDirectory(toPath + "\\" + strFolderName);
            }
            //创建数组保存源文件夹下的文件名
            var strFiles = Directory.GetFiles(fromPath);
            //循环拷贝文件
            foreach (var filename in strFiles)
            {
                //取得拷贝的文件名，只取文件名，地址截掉。
                var strFileName = filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1, filename.Length - filename.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                //开始拷贝文件,true表示覆盖同名文件
                File.Copy(filename, toPath + "\\" + strFolderName + "\\" + strFileName, true);
            }
            //创建DirectoryInfo实例
            var dirInfo = new DirectoryInfo(fromPath);
            //取得源文件夹下的所有子文件夹名称
            var ziPath = dirInfo.GetDirectories();
            foreach (DirectoryInfo ziDirectoryInfo in ziPath)
            {
                //获取所有子文件夹名
                string strZiPath = fromPath + "\\" + ziDirectoryInfo;
                //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                CopyDirectory(strZiPath, toPath + "\\" + strFolderName);
            }
        }

    }


}
