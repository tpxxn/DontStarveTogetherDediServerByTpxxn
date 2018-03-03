using System.Collections.Generic;

namespace 饥荒开服工具ByTpxxn.Class.DedicateServer
{

    /// <summary>
    /// 小细节的每一个选项<para/>
    /// 示例：{description = "Detect", data = "detect", hover = "Detect the language based on language mods installed."},
    /// </summary>
    public class Option
    {
        /// <summary>
        /// 选项的描述[显示项]
        /// </summary>
        public string Description;

        /// <summary>
        /// 选项的值[数据项]
        /// </summary>
        public string Data;

        /// <summary>
        /// 选项的解释[相当于Tip]
        /// </summary>
        public string Hover;
    }

    /// <summary>
    /// mod的单个设置
    /// </summary>
    public class ModSetting
    {
        #region modinfo.lua里有的字段

        /// <summary>
        /// 设置的名字[name字段，唯一识别标志，相当于对象名，modmain里用这个名字读取数据]
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设置的Label[label字段，标签]
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 设置的更多解释[相当于Tip]
        /// </summary>
        public string Hover { get; set; }

        /// <summary>
        /// 设置都有哪些选项[options字段]
        /// </summary>
        internal List<Option> Options { get; set; }

        /// <summary>
        /// 设置的默认值
        /// </summary>
        public string Default { get; set; }

        #endregion

        #region modinfo.lua里没有的字段[额外数据]

        /// <summary>
        /// 设置的当前值[先读默认值，之后用读取的当前值覆盖]
        /// </summary>
        public string Current { get; set; }

        /// <summary>
        /// 设置的当前值的翻译
        /// </summary>
        private string _currentDescription;

        public string CurrentDescription
        {
            get
            {
                foreach (var option in Options)
                {
                    if (option.Data == Current)
                    {
                        return option.Description;
                    }
                }
                return Current;
            }

        }

        #endregion
    }
}
