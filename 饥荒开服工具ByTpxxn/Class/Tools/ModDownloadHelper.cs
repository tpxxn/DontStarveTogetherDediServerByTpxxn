using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.View;

// 经作者同意，代码改自
// https://github.com/vvwall/DSTseverTools

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    public class ModDownloadObject
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string ModDescribe { get; set; }
        public string ModDownloadUrl { get; set; }
    }

    public static class ModDownloadHelper
    {
        public static ModDownloadObject DownloadModFromId(string modId)
        {
            var modDownloadObject = new ModDownloadObject();
            if (modId == "")
            {
                MessageBox.Show("请填写mod编号");
            }
            else
            {
                var postString = "mid=" + modId;
                var httpPostBack = HttpHelper.HttpPost("http://t.vvwall.com/DST/modinfo.php", postString);
                Debug.WriteLine("mod Json:"+httpPostBack);
                var jObject = JObject.Parse(httpPostBack);
                if (jObject["code"].ToString() == "200")
                {
                    modDownloadObject.ModId = jObject["mid"].ToString();
                    if (jObject["title"] != null)
                    {
                        modDownloadObject.ModName = jObject["title"].ToString();
                        if (jObject["des"] != null)
                        {
                            modDownloadObject.ModDescribe = jObject["des"].ToString();
                        }
                    }
                    if (jObject["url"] != null)
                    {
                        modDownloadObject.ModDownloadUrl = jObject["url"].ToString();
                    }
                }
                else
                {
                    MessageBox.Show(jObject["code"].ToString() == "404" ? "部分功能不能使用" : "未找到该mod");
                    return null;
                }
            }
            return modDownloadObject;
        }

        public static void DownloadModFile(ModDownloadObject modDownloadObject)
        {
            Directory.CreateDirectory(@".\Temp\ModUpdate\");
            HttpHelper.DownloadFile(modDownloadObject.ModDownloadUrl, @".\Temp\ModUpdate\workshop-" + modDownloadObject.ModId + ".zip");
        }
    }
}
