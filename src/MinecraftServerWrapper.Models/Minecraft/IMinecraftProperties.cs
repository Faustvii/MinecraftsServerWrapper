namespace MinecraftServerWrapper.Models.Minecraft
{
    public interface IMinecraftProperties
    {
        string Get(string field, string defValue);
        string Get(string field);
        int Get(string field, int defValue);
        bool Get(string field, bool defValue);
        void Set(string field, object value);
        void Reload();

        string LevelName { get; }
        string MessageOfTheDay { get; }
        int Port { get; }
        int MaxPlayers { get; }
    }
}
