using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT_Core.InfoItemClass
{

    /// <summary>
    /// 用于每个Component的标记信息
    /// </summary>
    public class TextureMarkUpInfo
    {
        /// <summary>
        /// 标记名称
        /// </summary>
        [JsonProperty("MarkName")]
        public string MarkName { get; set; } = "";

        /// <summary>
        /// 标记类型，分为Slot风格和Hash风格
        /// </summary>
        [JsonProperty("MarkType")]
        public string MarkType { get; set; } = "";

        /// <summary>
        /// 贴图本身的Hash值
        /// </summary>
        [JsonProperty("MarkHash")]
        public string MarkHash { get; set; } = "";

        /// <summary>
        /// 标记时所处的槽位
        /// </summary>
        [JsonProperty("MarkSlot")]
        public string MarkSlot { get; set; } = "";
        public string MarkFileName { get; set; } = "";


        public JObject GetJObject() {
            JObject jobj = new JObject();
            jobj["MarkName"] = MarkName;
            jobj["MarkType"] = MarkType;
            jobj["MarkHash"] = MarkHash;
            jobj["MarkSlot"] = MarkSlot;
            jobj["MarkFileName"] = MarkFileName;
            return jobj;
        }

    }
}
