using System.Collections.Generic;
using Newtonsoft.Json;

namespace 饥荒开服工具ByTpxxn.Class.JsonDeserialize.Hanization
{
    public class World
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Resources
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Foods
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Animals
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Monsters
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

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
        public Master Master { get; set; }
        public Caves Caves { get; set; }

        public Hanization()
        {
            Master = new Master();
            Caves = new Caves();
        }
    }

    public class HanizationRootObject
    {
        public Hanization Hanization { get; set; }

        public HanizationRootObject()
        {
            Hanization = new Hanization();
        }
    }
}
