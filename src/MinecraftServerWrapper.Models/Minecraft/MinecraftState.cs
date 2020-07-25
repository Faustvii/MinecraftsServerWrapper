using System;
using System.Collections.Generic;

namespace MinecraftServerWrapper.Models.Minecraft
{
    public class MinecraftState
    {
        public bool Running { get; set; }
        public int Port { get; private set; } = 25565;
        public int MaxPlayerCount { get; private set; } = 20;
        public int CurrentPlayerCount => CurrentPlayers.Count;
        public string LevelName { get; set; }
        public List<string> CurrentPlayers { get; private set; } = new List<string>();

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? LastBackup { get; set; }

        private readonly MinecraftProperties _minecraftProperties;

        public MinecraftState(MinecraftProperties minecraftProperties)
        {
            _minecraftProperties = minecraftProperties;
            LoadProperties();
        }

        public void ReloadMinecraftProperties()
        {
            _minecraftProperties?.Reload();
            LoadProperties();
        }

        private void LoadProperties()
        {
            if (_minecraftProperties != null)
            {
                MaxPlayerCount = _minecraftProperties.MaxPlayers;
                Port = _minecraftProperties.Port;
                LevelName = _minecraftProperties.LevelName;
            }
        }
    }
}
