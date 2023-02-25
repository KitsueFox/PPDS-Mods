using MelonLoader;

#nullable disable

namespace Duck_Trainer
{

    public static class DuckTrainerSettings //MelonSettings for the Mod
    {
        private const string SettingsCategory = "Duck Trainer";
        internal static MelonPreferences_Entry<bool> AutoRespawn;
        internal static MelonPreferences_Entry<bool> Achievements;

        internal static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory(SettingsCategory, "Duck Trainer");

            AutoRespawn = category.CreateEntry("AutoRespawn", false, "Auto Respawn Setting",
                "Ducks will Auto Respawn and won't be removed from the Level.");
            Achievements = category.CreateEntry("Achievements", false, "In-game Achievements",
                "Allow Achieving in-game Achievements \n(Restart game to take effect)");
        }
    }
}