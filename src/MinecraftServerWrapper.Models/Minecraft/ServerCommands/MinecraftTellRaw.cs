using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinecraftServerWrapper.Models.Minecraft.ServerCommands
{
    public class MinecraftTellRaw
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("color")]
        public MinecraftColor Color { get; set; }

        [JsonPropertyName("bold")]
        public bool Bold { get; set; }

        [JsonPropertyName("italic")]
        public bool Italic { get; set; }

        [JsonPropertyName("underlined")]
        public bool Underlined { get; set; }

        [JsonPropertyName("obfuscated")]
        public bool Obfuscated { get; set; }

        [JsonPropertyName("strikethrough")]
        public bool Strikethrough { get; set; }

        public MinecraftTellRaw(string text = "", MinecraftColor color = MinecraftColor.white, bool bold = false, bool italic = false,
            bool underlined = false, bool obfuscated = false, bool strikethrough = false)
        {
            Text = text;
            Color = color;
            Bold = bold;
            Italic = italic;
            Underlined = underlined;
            Obfuscated = obfuscated;
            Strikethrough = strikethrough;
        }

        /// <summary>
        /// Forms a tellraw command from the given arguments.
        /// See https://github.com/skylinerw/guides/blob/master/java/text%20component.md
        /// </summary>
        /// <returns></returns>
        public string ToCommand(string selector = "@a")
        {
            return $"tellraw {selector} {JsonSerializer.Serialize(this)}";
        }
    }
}
