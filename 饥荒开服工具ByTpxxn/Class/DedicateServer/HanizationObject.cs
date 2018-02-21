using System.Collections.Generic;
using Newtonsoft.Json;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{
    public class HanizationItem
    {
        public string Key { get; set; }
        public string KeyHanization { get; set; }
        public List<string> ValueHanization { get; set; }

        public HanizationItem()
        {
            ValueHanization = new List<string>();
        }
    }

    public class World : HanizationItem { }

    public class Resources : HanizationItem { }

    public class Foods : HanizationItem { }

    public class Animals : HanizationItem { }

    public class Monsters : HanizationItem { }

    public class Master
    {
        [JsonProperty("world")]
        public List<World> World { get; set; }
        [JsonProperty("resources")]
        public List<Resources> Resources { get; set; }
        [JsonProperty("foods")]
        public List<Foods> Foods { get; set; }
        [JsonProperty("animals")]
        public List<Animals> Animals { get; set; }
        [JsonProperty("monsters")]
        public List<Monsters> Monsters { get; set; }

        public Master()
        {
            World = new List<World>();
            Resources = new List<Resources>();
            Foods = new List<Foods>();
            Animals = new List<Animals>();
            Monsters = new List<Monsters>();
        }
    }

    public class Caves
    {
        [JsonProperty("world")]
        public List<World> World { get; set; }
        [JsonProperty("resources")]
        public List<Resources> Resources { get; set; }
        [JsonProperty("foods")]
        public List<Foods> Foods { get; set; }
        [JsonProperty("animals")]
        public List<Animals> Animals { get; set; }
        [JsonProperty("monsters")]
        public List<Monsters> Monsters { get; set; }

        public Caves()
        {
            World = new List<World>();
            Resources = new List<Resources>();
            Foods = new List<Foods>();
            Animals = new List<Animals>();
            Monsters = new List<Monsters>();
        }
    }

    public class Hanization
    {
        [JsonProperty("Master")]
        public Master Master { get; set; }
        [JsonProperty("Caves")]
        public Caves Caves { get; set; }

        public Hanization()
        {
            Master = new Master();
            Caves = new Caves();
        }
    }

    public class HanizationObject
    {
        public Hanization Hanization { get; set; }

        public HanizationObject()
        {
            Hanization = new Hanization();
        }
    }
}
