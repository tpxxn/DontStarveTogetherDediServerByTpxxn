using System.Collections.Generic;
using Newtonsoft.Json;

namespace 饥荒开服工具ByTpxxn.Class.JsonDeserialize.EditWorld
{
    public class EditWorldItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class World : EditWorldItem { }

    public class Resources : EditWorldItem { }

    public class Foods : EditWorldItem { }

    public class Animals : EditWorldItem { }

    public class Monsters : EditWorldItem { }

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

    public class EditWorld
    {
        public Master Master { get; set; }
        public Caves Caves { get; set; }

        public EditWorld()
        {
            Master = new Master();
            Caves = new Caves();
        }
    }

    public class EditWorldRootObject
    {
        public EditWorld EditWorld { get; set; }

        public EditWorldRootObject()
        {
            EditWorld = new EditWorld();
        }
    }
}
