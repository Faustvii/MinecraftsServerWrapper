using System.IO;
using System.Text.Json;

namespace MinecraftServerWrapper.Utilities
{
    public class SettingsManager<T> where T : class, new()
    {
        private readonly string _filePath;

        public SettingsManager(string fileName)
        {
            _filePath = GetLocalFilePath(fileName);
        }

        private string GetLocalFilePath(string fileName)
        {
            return fileName;
        }

        public T LoadSettings() => File.Exists(_filePath) ? JsonSerializer.Deserialize<T>(File.ReadAllText(_filePath)) : new T();

        public void SaveSettings(T settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_filePath, json);
        }
    }
}
