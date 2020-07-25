using Microsoft.Extensions.Options;
using MinecraftServerWrapper.Models.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinecraftServerWrapper.Models.Minecraft
{
    public class MinecraftProperties : IMinecraftProperties
    {
        private Dictionary<string, string> Props;
        private string Filename;

        public string LevelName => Get("level-name", "world");
        public string MessageOfTheDay => Get("motd", string.Empty);
        public int Port => Get("server-port", 25565);
        public int MaxPlayers => Get("max-players", 20);

        public MinecraftProperties(IOptions<MinecraftSettings> options)
        {
            var file = Path.Combine(options.Value.ServerFolderPath, "server.properties");
            Reload(file);
        }

        public string Get(string field, string defValue)
        {
            return Get(field) ?? defValue;
        }

        public int Get(string field, int defValue)
        {
            var val = Get(field);
            if (val == null)
                return defValue;
            if (int.TryParse(val, out var result))
            {
                return result;
            }
            return defValue;
        }

        public bool Get(string field, bool defValue)
        {
            var val = Get(field);
            if (val == null)
                return defValue;
            if (bool.TryParse(val, out var result))
            {
                return result;
            }
            return defValue;
        }

        public string Get(string field)
        {
            return Props.ContainsKey(field) ? Props[field] : (null);
        }

        public void Set(string field, object value)
        {
            if (!Props.ContainsKey(field))
            {
                Props.Add(field, value.ToString());
            }
            else
            {
                Props[field] = value.ToString();
            }
        }

        public void Save()
        {
            Save(Filename);
        }

        public void Save(string filename)
        {
            Filename = filename;

            if (!File.Exists(filename))
            {
                File.Create(filename);
            }

            var file = new StreamWriter(filename);

            foreach (var prop in Props.Keys.ToArray())
            {
                if (!string.IsNullOrWhiteSpace(Props[prop]))
                    file.WriteLine(prop + "=" + Props[prop]);
            }

            file.Close();
        }

        public void Reload()
        {
            Reload(Filename);
        }

        public void Reload(string filename)
        {
            Filename = filename;
            Props = new Dictionary<string, string>();

            if (File.Exists(filename))
            {
                LoadFromFile(filename);
            }
            else
            {
                File.Create(filename);
            }
        }

        private void LoadFromFile(string file)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    var index = line.IndexOf('=');
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value[1..^1];
                    }

                    try
                    {
                        //ignore dublicates
                        Props.Add(key, value);
                    }
                    catch { }
                }
            }
        }
    }
}
