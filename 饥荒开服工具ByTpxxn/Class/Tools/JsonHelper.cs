using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize;
using 饥荒开服工具ByTpxxn.Class;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize.Hanization;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal class JsonHelper
    {
        #region 读取汉化

        /// <summary>
        /// 读取汉化
        /// </summary>
        public static HanizationObject ReadHanization()
        {
            var serverConfig = JsonConvert.DeserializeObject<HanizationRootObject>(StringProcess.GetJsonString("EditWorld/Hanization.json"));
            var master = serverConfig.Hanization.Master;
            var caves = serverConfig.Hanization.Caves;
            var hanizationObject = new HanizationObject();
            foreach (var item in master.World)
            {
                hanizationObject.Hanization.Master.World.Add(new DedicateServer.World { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in master.Resources)
            {
                hanizationObject.Hanization.Master.Resources.Add(new DedicateServer.Resources { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in master.Foods)
            {
                hanizationObject.Hanization.Master.Foods.Add(new DedicateServer.Foods { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in master.Animals)
            {
                hanizationObject.Hanization.Master.Animals.Add(new DedicateServer.Animals { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in master.Monsters)
            {
                hanizationObject.Hanization.Master.Monsters.Add(new DedicateServer.Monsters { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in caves.World)
            {
                hanizationObject.Hanization.Master.World.Add(new DedicateServer.World { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in caves.Resources)
            {
                hanizationObject.Hanization.Master.Resources.Add(new DedicateServer.Resources { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in caves.Foods)
            {
                hanizationObject.Hanization.Master.Foods.Add(new DedicateServer.Foods { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in caves.Animals)
            {
                hanizationObject.Hanization.Master.Animals.Add(new DedicateServer.Animals { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            foreach (var item in caves.Monsters)
            {
                hanizationObject.Hanization.Master.Monsters.Add(new DedicateServer.Monsters { Key = item.Key, KeyHanization = item.KeyHanization, ValueHanization = item.ValueHanization.Split(',').ToList() });
            }
            return hanizationObject;
        }

        #endregion
        
    }
}
