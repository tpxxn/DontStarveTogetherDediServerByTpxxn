using System.Collections.Generic;
using Newtonsoft.Json;

namespace 饥荒开服工具ByTpxxn.Class.JsonDeserialize.Preset
{
    public class World
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Resources
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Foods
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Animals
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Monsters
    {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Preset
    {
        public string PresetType { get; set; }
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

        public Preset()
        {
            World = new List<World>();
            Resources = new List<Resources>();
            Foods = new List<Foods>();
            Animals = new List<Animals>();
            Monsters = new List<Monsters>();
        }
    }

    public class PresetRootObject
    {
        public Preset Preset { get; set; }

        public PresetRootObject()
        {
            Preset = new Preset();
        }
    }
}
