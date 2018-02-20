using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        /// <summary>
        /// 是否开启洞穴
        /// </summary>
        private void EditWorldIsCaveSelectBox_SelectionChanged()
        {
            CaveSettingColumnDefinition.Width = EditWorldIsCaveSelectBox.Text == "开启" ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }

        private static int ReturnIndex(string str,List<string> stringList)
        {
            for(var i = 0;i<stringList.Count;i++)
            {
                if (str == stringList[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 设置"地上世界"
        /// </summary>
        private void SetOverWorldSet()
        {
            // 地上 
            _overWorld = new Leveldataoverride(_dediFilePath, false);
            // 清空界面
            DediOverWorldWorld.Children.Clear();
            DediOverWolrdFoods.Children.Clear();
            DediOverWorldAnimals.Children.Clear();
            DediOverWorldMonsters.Children.Clear();
            DediOverWorldResources.Children.Clear();
            // 地上 分类
            var worldClassification = JsonHelper.ReadWorldClassification(false);
            var foods = new Dictionary<string, EditWorldItem>();
            var animals = new Dictionary<string, EditWorldItem>();
            var monsters = new Dictionary<string, EditWorldItem>();
            var resources = new Dictionary<string, EditWorldItem>();
            var world = new Dictionary<string, EditWorldItem>();
            #region 地上分类方法
            foreach (var item in _overWorld.FinalWorldDictionary)
            {
                if (worldClassification.ContainsKey(item.Key))
                {
                    switch (worldClassification[item.Key])
                    {
                        case "world":
                            world[item.Key] = item.Value;
                            break;
                        case "resources":
                            resources[item.Key] = item.Value;
                            break;
                        case "foods":
                            foods[item.Key] = item.Value;
                            break;
                        case "animals":
                            animals[item.Key] = item.Value;
                            break;
                        case "monsters":
                            monsters[item.Key] = item.Value;
                            break;
                    }
                }
                else
                {
                    world[item.Key] = item.Value;
                }
            }
            #endregion
            #region "显示" 地上
            foreach (var item in world)
            {
                if (item.Value.ToolTip == "roads" || item.Value.ToolTip == "layout_mode" || item.Value.ToolTip == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    TextList = Hanization(item.Value.WorldConfigList),
                    TextIndex = ReturnIndex(Hanization(item.Value.WorldConfig), Hanization(item.Value.WorldConfigList)),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                var comboBoxWithImage = new DediComboBoxWithImage()
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60

                };
                comboBoxWithImage.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldWorld.Children.Add(dediEditWorldSelectBox);
                DediOverWorldWorld.Children.Add(comboBoxWithImage);
            }
            foreach (var item in foods)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWolrdFoods.Children.Add(comboBoxWithImage);

            }
            foreach (var item in animals)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldAnimals.Children.Add(comboBoxWithImage);

            }
            foreach (var item in monsters)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldMonsters.Children.Add(comboBoxWithImage);

            }
            foreach (var item in resources)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldResources.Children.Add(comboBoxWithImage);

            }
            #endregion
        }

        /// <summary>
        /// 设置"地上世界"(测试 用)
        /// </summary>
        private void DiOverWorld_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dedi = (DediComboBoxWithImage)sender;
            // List<string> s = new List<string>();
            // s.Add("tag:" + Dedi.Tag.ToString());
            // s.Add("e.source:" + e.Source.ToString());
            // s.Add(e.AddedItems.Count.ToString());
            // s.Add(e.RemovedItems.Count.ToString());
            // s.Add(Dedi.SelectedIndex.ToString());
            // foreach (var item in s)
            // {
            //     Debug.WriteLine(item);
            // }
            // 此时说明修改
            if (e.RemovedItems.Count != 0 && e.AddedItems[0].ToString() == Hanization(_overWorld.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfigList[dedi.SelectedIndex]))
            {
                _overWorld.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfig = _overWorld.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfigList[dedi.SelectedIndex];
                Debug.WriteLine(dedi.Tag + "选项变为:" + _overWorld.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfig);
                // 保存,这样保存有点卡,换为每次点击radioButton或创建世界时
                //OverWorld.SaveWorld();
                //Debug.WriteLine("保存地上世界");
            }
        }

        /// <summary>
        /// 设置"地下世界"
        /// </summary>
        private void SetCavesSet()
        {
            // 地下
            _caves = new Leveldataoverride(_dediFilePath, true);
            // 清空界面
            DediCavesWorld.Children.Clear();
            DediCavesFoods.Children.Clear();
            DediCavesAnimals.Children.Clear();
            DediCavesMonsters.Children.Clear();
            DediCavesResources.Children.Clear();
            // 地下 分类
            var worldClassification = JsonHelper.ReadWorldClassification(true);
            var foods = new Dictionary<string, EditWorldItem>();
            var animals = new Dictionary<string, EditWorldItem>();
            var monsters = new Dictionary<string, EditWorldItem>();
            var resources = new Dictionary<string, EditWorldItem>();
            var world = new Dictionary<string, EditWorldItem>();
            #region  地下分类方法
            foreach (var item in _caves.FinalWorldDictionary)
            {
                if (worldClassification.ContainsKey(item.Key))
                {
                    switch (worldClassification[item.Key])
                    {
                        case "world":
                            world[item.Key] = item.Value;
                            break;
                        case "resources":
                            resources[item.Key] = item.Value;
                            break;
                        case "foods":
                            foods[item.Key] = item.Value;
                            break;
                        case "animals":
                            animals[item.Key] = item.Value;
                            break;
                        case "monsters":
                            monsters[item.Key] = item.Value;
                            break;
                    }
                }
                else
                {
                    world[item.Key] = item.Value;
                }
            }
            #endregion
            #region "显示" 地下
            // animals
            foreach (var item in world)
            {
                if (item.Value.ToolTip == "roads" || item.Value.ToolTip == "layout_mode" || item.Value.ToolTip == "wormhole_prefab")
                {
                    continue;
                }
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesWorld.Children.Add(comboBoxWithImage);
            }
            foreach (var item in foods)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesFoods.Children.Add(comboBoxWithImage);

            }
            foreach (var item in animals)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesAnimals.Children.Add(comboBoxWithImage);

            }
            foreach (var item in monsters)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesMonsters.Children.Add(comboBoxWithImage);

            }
            foreach (var item in resources)
            {
                var comboBoxWithImage = new DediComboBoxWithImage
                {
                    ImageSource = new BitmapImage(new Uri("/" + item.Value.PicturePath, UriKind.Relative)),
                    ItemsSource = Hanization(item.Value.WorldConfigList),
                    SelectedValue = Hanization(item.Value.WorldConfig),
                    ImageToolTip = Hanization(item.Value.ToolTip),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                comboBoxWithImage.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesResources.Children.Add(comboBoxWithImage);

            }
            #endregion
        }

        /// <summary>
        /// 设置"地下世界"(测试 用)
        /// </summary>
        private void DiCaves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dedi = (DediComboBoxWithImage)sender;
            // 此时说明修改
            if (e.RemovedItems.Count != 0 && e.AddedItems[0].ToString() == Hanization(_caves.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfigList[dedi.SelectedIndex]))
            {
                _caves.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfig = _caves.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfigList[dedi.SelectedIndex];
                Debug.WriteLine(dedi.Tag + "选项变为:" + _caves.FinalWorldDictionary[dedi.Tag.ToString()].WorldConfig);
                // 保存,这样保存有点卡,换为每次点击radioButton或创建世界时
                //Caves.SaveWorld();
                //Debug.WriteLine("保存地上世界");
            }
        }
    }
}
