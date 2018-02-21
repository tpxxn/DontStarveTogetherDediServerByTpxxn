using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    /// <summary>
    /// LeveldataOverride项的公共基类
    /// </summary>
    public class LeveldataOverrideItem
    {
        public string Picture { get; set; }
        public string Key { get; set; }
        public List<string> Value { get; set; }
        public int Index { get; set; }

        public LeveldataOverrideItem()
        {
            Value = new List<string>();
        }
    }

    public class LeveldataoverrideWorld : LeveldataOverrideItem { }

    public class LeveldataoverrideResources : LeveldataOverrideItem { }
    
    public class LeveldataoverrideFoods : LeveldataOverrideItem { }
    
    public class LeveldataoverrideAnimals : LeveldataOverrideItem { }
    
    public class LeveldataoverrideMonsters : LeveldataOverrideItem { }
    
    internal class LeveldataoverrideObject
    {
        public List<LeveldataoverrideWorld> World { get; set; }
        public List<LeveldataoverrideResources> Resources { get; set; }
        public List<LeveldataoverrideFoods> Foods { get; set; }
        public List<LeveldataoverrideAnimals> Animals { get; set; }
        public List<LeveldataoverrideMonsters> Monsters { get; set; }

        public LeveldataoverrideObject()
        {
            World = new List<LeveldataoverrideWorld>();
            Resources = new List<LeveldataoverrideResources>();
            Foods = new List<LeveldataoverrideFoods>();
            Animals = new List<LeveldataoverrideAnimals>();
            Monsters = new List<LeveldataoverrideMonsters>();
        }
    }

}
