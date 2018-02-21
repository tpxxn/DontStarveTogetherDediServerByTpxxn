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

        /// <summary>
        /// 设置"地上世界"
        /// </summary>
        private void SetOverWorldSet()
        {
            // 地上 
            _overWorld = new Leveldataoverride(_dediFilePath, false);
            var leveldataOverrideObject = _overWorld._leveldataOverrideObject;
            // 清空界面
            DediOverWorldWorld.Children.Clear();
            DediOverWorldResources.Children.Clear();
            DediOverWorldFoods.Children.Clear();
            DediOverWorldAnimals.Children.Clear();
            DediOverWorldMonsters.Children.Clear();
            // "显示" 地上
            foreach (var item in leveldataOverrideObject.World)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldWorld.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Resources)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldResources.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Foods)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldFoods.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Animals)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldAnimals.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Monsters)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiOverWorld_SelectionChanged;
                DediOverWorldMonsters.Children.Add(dediEditWorldSelectBox);
            }
        }

        /// <summary>
        /// 设置"地上世界"
        /// </summary>
        private void DiOverWorld_SelectionChanged(object sender)
        {
            var dediEditWorldSelectBox = (DediEditWorldSelectBox)sender;
            foreach (var item in _overWorld._leveldataOverrideObject.World)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _overWorld._leveldataOverrideObject.Resources)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _overWorld._leveldataOverrideObject.Foods)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _overWorld._leveldataOverrideObject.Animals)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _overWorld._leveldataOverrideObject.Monsters)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
        }

        /// <summary>
        /// 设置"地下世界"
        /// </summary>
        private void SetCavesSet()
        {
            // 地下
            _caves = new Leveldataoverride(_dediFilePath, true);
            var leveldataOverrideObject = _caves._leveldataOverrideObject;
            // 清空界面
            DediCavesWorld.Children.Clear();
            DediCavesFoods.Children.Clear();
            DediCavesAnimals.Children.Clear();
            DediCavesMonsters.Children.Clear();
            DediCavesResources.Children.Clear();
            #region "显示" 地下
            // animals
            foreach (var item in leveldataOverrideObject.World)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesWorld.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Resources)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesResources.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Foods)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesFoods.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Animals)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesAnimals.Children.Add(dediEditWorldSelectBox);
            }
            foreach (var item in leveldataOverrideObject.Monsters)
            {
                if (item.Key == "roads" || item.Key == "layout_mode" || item.Key == "wormhole_prefab")
                {
                    continue;
                }
                var dediEditWorldSelectBox = new DediEditWorldSelectBox
                {
                    ImageSource = new BitmapImage(new Uri(item.Picture, UriKind.Relative)),
                    TextList = HanizationList(item.Key),
                    TextIndex = item.Index,
                    ImageToolTip = Hanization(item.Key),
                    Tag = item.Key,
                    Width = 200,
                    Height = 60
                };
                dediEditWorldSelectBox.SelectionChanged += DiCaves_SelectionChanged;
                DediCavesMonsters.Children.Add(dediEditWorldSelectBox);
            }
            #endregion
        }

        /// <summary>
        /// 设置"地下世界"
        /// </summary>
        private void DiCaves_SelectionChanged(object sender)
        {
            var dediEditWorldSelectBox = (DediEditWorldSelectBox)sender;
            foreach (var item in _caves._leveldataOverrideObject.World)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _caves._leveldataOverrideObject.Resources)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _caves._leveldataOverrideObject.Foods)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _caves._leveldataOverrideObject.Animals)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
            foreach (var item in _caves._leveldataOverrideObject.Monsters)
            {
                if (Hanization(item.Key) == dediEditWorldSelectBox.TitleText)
                {
                    item.Index = dediEditWorldSelectBox.TextIndex;
                }
            }
        }
        
    }
}
