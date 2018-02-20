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
            var streamReader = new StreamReader(Global.GetStreamFromProjectFile(path), Global.Utf8WithoutBom);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// [点击左侧RadioButton时]复制ServerTemplate到指定位置
        /// </summary>
        public static void CopyServerTemplateFile()
        {
            // 判断是否存在
            if (Directory.Exists(CommonPath.SaveRootDirPath + @"\Server"))
            {
                Directory.Delete(CommonPath.SaveRootDirPath + @"\Server", true);
            }
            // 建立文件夹
            Directory.CreateDirectory(CommonPath.SaveRootDirPath + @"\Server");
            Directory.CreateDirectory(CommonPath.SaveRootDirPath + @"\Server\Caves");
            Directory.CreateDirectory(CommonPath.SaveRootDirPath + @"\Server\Master");
            // 填文件[公共文件]
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\cluster.ini", ReadResources("ServerTemplate.cluster.ini"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Caves\leveldataoverride.lua", ReadResources("ServerTemplate.Caves.leveldataoverride.lua"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Caves\modoverrides.lua", ReadResources("ServerTemplate.Caves.modoverrides.lua"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Caves\server.ini", ReadResources("ServerTemplate.Caves.server.ini"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Master\leveldataoverride.lua", ReadResources("ServerTemplate.Master.leveldataoverride.lua"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Master\modoverrides.lua", ReadResources("ServerTemplate.Master.modoverrides.lua"), Global.Utf8WithoutBom);
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\Master\server.ini", ReadResources("ServerTemplate.Master.server.ini"), Global.Utf8WithoutBom);
            // 填文件[ClusterToken]
            File.WriteAllText(CommonPath.SaveRootDirPath + @"\Server\cluster_token.txt",!string.IsNullOrEmpty(CommonPath.ClusterToken)? CommonPath.ClusterToken: "",Global.Utf8WithoutBom);
        }

        public static void RenameServerTemplateFile(int saveSlot)
        {
            if (!Directory.Exists(CommonPath.SaveRootDirPath + @"\DedicatedServer_" + saveSlot))
            {
                Directory.Move(CommonPath.SaveRootDirPath + @"\Server", CommonPath.SaveRootDirPath + @"\DedicatedServer_" + saveSlot);
                // 删除临时文件
                if (Directory.Exists(CommonPath.SaveRootDirPath + @"\Server"))
                {
                    Directory.Delete(CommonPath.SaveRootDirPath + @"\Server", true);
                }
            }
        }
    }
}
