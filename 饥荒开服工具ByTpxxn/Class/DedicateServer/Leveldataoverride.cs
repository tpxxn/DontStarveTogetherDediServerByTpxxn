using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using 饥荒开服工具ByTpxxn.Class.JsonDeserialize.EditWorld;
using 饥荒开服工具ByTpxxn.Class.Tools;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    /// <summary>
    /// 世界类
    /// </summary>
    internal class Leveldataoverride
    {
        #region 字段和属性

        private readonly DediFilePath _pathall;

        /// <summary>
        /// 是否为洞穴
        /// </summary>
        private readonly bool _isCave;

        /// <summary>
        /// 读取的选项对象
        /// </summary>
        public LeveldataoverrideObject _leveldataOverrideObject { get; set; }

        /// <summary>
        /// 从lua中读取的所选项
        /// </summary>
        public Dictionary<string, string> _luaSelectedDictionary { get; set; }
        
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pathall">所有路径</param>
        /// <param name="isCave">是否为洞穴</param>
        public Leveldataoverride(DediFilePath pathall, bool isCave)
        {
            _pathall = pathall;
            _isCave = isCave;
            // 初始化，就是，读取地上地下世界，放到 Dictionary<string（世界的key）,List<string>（世界的value）> 类型中，
            // 但是以后如何在里面取值赋值
            Debug.WriteLine("初始化结果：" + Init()); 
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>返回初始化成功文本</returns>
        private string Init()
        {
            if (string.IsNullOrEmpty(_pathall.CaveConfigFilePath))
            {
                return "世界配置文件路径不对";
            }
            // 读取[世界选项]值
            _luaSelectedDictionary = ReadEditWorldFromLua();
            // 给[世界选项文件]初始化
            ReadEditWorldJson();
            return "初始化成功";
        }

        #region Redux方法

        /// <summary>
        /// 读取世界选项的文件,赋值到selectConfigWorld，类型
        /// </summary>
        private void ReadEditWorldJson()
        {
            // 先清空,再赋值
            _leveldataOverrideObject = new LeveldataoverrideObject();
            // 读取文件,填入到字典
            var serverConfig = JsonConvert.DeserializeObject<EditWorldRootObject>(StringProcess.GetJsonString("EditWorld/EditWorld.json"));
            if (!_isCave)
            {
                var master = serverConfig.EditWorld.Master;
                foreach (var world in master.World)
                {
                    _leveldataOverrideObject.World.Add(new LeveldataoverrideWorld
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + world.Key + ".png",
                        Key = world.Key,
                        Value = world.Value.Split(',').ToList()
                    });
                }
                foreach (var resources in master.Resources)
                {
                    _leveldataOverrideObject.Resources.Add(new LeveldataoverrideResources
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + resources.Key + ".png",
                        Key = resources.Key,
                        Value = resources.Value.Split(',').ToList()
                    });
                }
                foreach (var foods in master.Foods)
                {
                    _leveldataOverrideObject.Foods.Add(new LeveldataoverrideFoods
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + foods.Key + ".png",
                        Key = foods.Key,
                        Value = foods.Value.Split(',').ToList()
                    });
                }
                foreach (var animals in master.Animals)
                {
                    _leveldataOverrideObject.Animals.Add(new LeveldataoverrideAnimals
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + animals.Key + ".png",
                        Key = animals.Key,
                        Value = animals.Value.Split(',').ToList()
                    });
                }
                foreach (var monsters in master.Monsters)
                {
                    _leveldataOverrideObject.Monsters.Add(new LeveldataoverrideMonsters
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + monsters.Key + ".png",
                        Key = monsters.Key,
                        Value = monsters.Value.Split(',').ToList()
                    });
                }
                foreach (var world in _leveldataOverrideObject.World)
                {
                    world.Index = GetIndex(world);
                }
                foreach (var resources in _leveldataOverrideObject.Resources)
                {
                    resources.Index = GetIndex(resources);
                }
                foreach (var foods in _leveldataOverrideObject.Foods)
                {
                    foods.Index = GetIndex(foods);
                }
                foreach (var animals in _leveldataOverrideObject.Animals)
                {
                    animals.Index = GetIndex(animals);
                }
                foreach (var monsters in _leveldataOverrideObject.Monsters)
                {
                    monsters.Index = GetIndex(monsters);
                }
            }
            else
            {
                var caves = serverConfig.EditWorld.Caves;
                foreach (var world in caves.World)
                {
                    _leveldataOverrideObject.World.Add(new LeveldataoverrideWorld
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + world.Key + ".png",
                        Key = world.Key,
                        Value = world.Value.Split(',').ToList()
                    });
                }
                foreach (var resources in caves.Resources)
                {
                    _leveldataOverrideObject.Resources.Add(new LeveldataoverrideResources
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + resources.Key + ".png",
                        Key = resources.Key,
                        Value = resources.Value.Split(',').ToList()
                    });
                }
                foreach (var foods in caves.Foods)
                {
                    _leveldataOverrideObject.Foods.Add(new LeveldataoverrideFoods
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + foods.Key + ".png",
                        Key = foods.Key,
                        Value = foods.Value.Split(',').ToList()
                    });
                }
                foreach (var animals in caves.Animals)
                {
                    _leveldataOverrideObject.Animals.Add(new LeveldataoverrideAnimals
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + animals.Key + ".png",
                        Key = animals.Key,
                        Value = animals.Value.Split(',').ToList()
                    });
                }
                foreach (var monsters in caves.Monsters)
                {
                    _leveldataOverrideObject.Monsters.Add(new LeveldataoverrideMonsters
                    {
                        Picture = @"/Resources/DedicatedServer/World/" + monsters.Key + ".png",
                        Key = monsters.Key,
                        Value = monsters.Value.Split(',').ToList()
                    });
                }
                foreach (var world in _leveldataOverrideObject.World)
                {
                    world.Index = GetIndex(world);
                }
                foreach (var resources in _leveldataOverrideObject.Resources)
                {
                    resources.Index = GetIndex(resources);
                }
                foreach (var foods in _leveldataOverrideObject.Foods)
                {
                    foods.Index = GetIndex(foods);
                }
                foreach (var animals in _leveldataOverrideObject.Animals)
                {
                    animals.Index = GetIndex(animals);
                }
                foreach (var monsters in _leveldataOverrideObject.Monsters)
                {
                    monsters.Index = GetIndex(monsters);
                }
            }
        }
        
        /// <summary>
        /// 读取世界配置文件，赋值到configWorld
        /// </summary>
        private Dictionary<string, string> ReadEditWorldFromLua()
        {
            // 清空
            var configWorld = new Dictionary<string, string>();
            // 先读文件
            var luaTable = LuaHelper.ReadLua(!_isCave ? _pathall.OverworldConfigFilePath : _pathall.CaveConfigFilePath, Encoding.UTF8, true);
            // 初始化override世界配置
            var overridesDictionary = ((LuaTable)luaTable["overrides"]).Members;
            var keyList = overridesDictionary.Keys.ToList();
            var valuesList = overridesDictionary.Values.ToList();
            for (var i = 0; i < overridesDictionary.Count; i++)
            {
                configWorld.Add(keyList[i], valuesList[i].ToString());
            }
            return configWorld;
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <returns></returns>
        private int GetIndex(LeveldataOverrideItem leveldataOverrideItem)
        {
            for (var i = 0; i < leveldataOverrideItem.Value.Count; i++)
            {
                if (_luaSelectedDictionary[leveldataOverrideItem.Key] == leveldataOverrideItem.Value[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void SaveWorld()
        {
            if (!Directory.Exists(_pathall.ServerDirPath))
            {
                Debug.WriteLine("Server路径不存在！");
                return;
            }
            // 保存到文件
            string savePath;
            if (!_isCave)
            {
                savePath = _pathall.ServerDirPath + @"\Master\leveldataoverride.lua";
            }
            else
            {
                savePath = _pathall.ServerDirPath + @"\Caves\leveldataoverride.lua";
            }
            var fileStream = new FileStream(savePath, FileMode.Create);
            var streamWriter = new StreamWriter(fileStream, Global.Utf8WithoutBom);
            // 开始写入
            var stringBuilder = new StringBuilder();
            // 地上世界
            // 读取模板中地上世界配置的前半部分和后半部分dishangStrQ，dishangStrH，用于拼接字符串，保存用
            string strQ, strH;
            if (!_isCave)
            {
                var masterWorld = FileHelper.ReadResources("ServerTemplate.Master.leveldataoverride.lua");
                masterWorld = masterWorld.Replace("\r\n", "\n").Replace("\n", "\r\n");
                var masterRegex = new Regex(@".*overrides\s*=\s*{|random_set_pieces.*", RegexOptions.Singleline);
                var masterMatchCollection = masterRegex.Matches(masterWorld);
                strQ = masterMatchCollection[0].Value + "\r\n";
                strH = "\r\n},\r\n" + masterMatchCollection[1].Value + "\r\n";
            }
            else
            {
                var cavesWorld = FileHelper.ReadResources("ServerTemplate.Caves.leveldataoverride.lua");
                cavesWorld = cavesWorld.Replace("\r\n", "\n").Replace("\n", "\r\n");
                var cavesRegex = new Regex(@".*overrides\s*=\s*{|required_prefabs.*", RegexOptions.Singleline);
                var cavesMatchCollection = cavesRegex.Matches(cavesWorld);
                strQ = cavesMatchCollection[0].Value + "\r\n";
                strH = "\r\n},\r\n" + cavesMatchCollection[1].Value + "\r\n";
            }
            // 追加前半部分
            stringBuilder.Append(strQ);
            // 追加中间部分
            var ParameterDictionary = new Dictionary<string, string>();
            foreach (var item in _leveldataOverrideObject.World)
            {
                ParameterDictionary.Add(item.Key, item.Value[item.Index]);
            }
            foreach (var item in _leveldataOverrideObject.Resources)
            {
                ParameterDictionary.Add(item.Key, item.Value[item.Index]);
            }
            foreach (var item in _leveldataOverrideObject.Foods)
            {
                ParameterDictionary.Add(item.Key, item.Value[item.Index]);
            }
            foreach (var item in _leveldataOverrideObject.Animals)
            {
                ParameterDictionary.Add(item.Key, item.Value[item.Index]);
            }
            foreach (var item in _leveldataOverrideObject.Monsters)
            {
                ParameterDictionary.Add(item.Key, item.Value[item.Index]);
            }
            if (!_isCave)
            {
                ParameterDictionary.Add("layout_mode", "LinkNodesByKeys");
                ParameterDictionary.Add("roads", "default");
                ParameterDictionary.Add("wormhole_prefab", "wormhole");
            }
            else
            {
                ParameterDictionary.Add("layout_mode", "RestrictNodesByKey");
                ParameterDictionary.Add("roads", "never");
                ParameterDictionary.Add("wormhole_prefab", "tentacle_pillar");
            }
            ParameterDictionary = ParameterDictionary.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value);
            foreach (var item in ParameterDictionary)
            {
                stringBuilder.AppendFormat("        {0}=\"{1}\",\r\n", item.Key, item.Value);
            }
            var str = stringBuilder.ToString();
            str = str.Substring(0, str.Length - 3);
            // 追加后半部分
            str += strH;
            streamWriter.Write(str);
            //清空缓冲区
            streamWriter.Flush();
            //关闭流
            streamWriter.Close();
            fileStream.Close();
        }

        #endregion
    }
}
