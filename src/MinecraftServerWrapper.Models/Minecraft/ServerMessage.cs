using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerWrapper.Models.Minecraft
{
    public class ServerMessage
    {
        public string Prefix { get; set; }
        public string Text { get; set; }
        public string RawText { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        public ServerMessage(string prefix, string text, string rawText)
        {
            Prefix = prefix;
            Text = text;
            RawText = rawText;
        }

        public static ServerMessage Create(string message, Dictionary<string, string> replacements = null)
        {
            var closeSquareBracket = message.IndexOf(']');
            if (closeSquareBracket < 0)
            {
                return new ServerMessage("", message, message);
            }
            var colon = message.Substring(closeSquareBracket).IndexOf(':') + closeSquareBracket;
            return colon < 0
                   ? new ServerMessage("", message, message)
                   : new ServerMessage(message.Substring(0, colon + 1).Trim(), message.Substring(colon + 2).Trim().ReplaceAll(replacements), message);
        }
    }

    internal static class ParserExtensions
    {
        /// <summary>
        /// Determines whether any given substring appears in the target <see cref="string" />
        /// </summary>
        /// <param name="str"></param>
        /// <param name="substrings">Substrings to check</param>
        public static bool ContainsAny(this string str, params string[] substrings) => substrings.Any(str.Contains);

        /// <summary>
        /// Replaces all keys in the provided dictionary with their associated values
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacements">A dictionary with values being the replacement for the associated keys</param>
        /// <returns></returns>
        public static string ReplaceAll(this string str, Dictionary<string, string> replacements)
        {
            if (replacements == null)
                return str;

            foreach (var pattern in replacements.Keys)
            {
                str = str.Replace(pattern, replacements[pattern]);
            }

            return str;
        }
    }
}
