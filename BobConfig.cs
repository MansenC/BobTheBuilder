using RedLoader;
using RedLoader.Preferences;

namespace BobTheBuilder
{
    internal static class BobConfig
    {
        public static ConfigCategory Category { get; private set; }

        public static KeybindConfigEntry SpawnKey { get; private set; }

        public static void Initialize()
        {
            Category = ConfigSystem.CreateFileCategory(
                nameof(BobTheBuilder),
                nameof(BobTheBuilder),
                $"{nameof(BobTheBuilder)}.cfg");

            SpawnKey = Category.CreateKeybindEntry(
                "spawn_key",
                EInputKey.o,
                "Spawn Test Cube",
                "Press for spawning the testing cube.");
        }
    }
}
