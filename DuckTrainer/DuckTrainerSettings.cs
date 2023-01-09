using MelonLoader;

#nullable disable

namespace Duck_Trainer
{
    
    public static class DuckTrainerSettings //MelonSettings for the Mod
    {
        private const string SettingsCategory = "Duck Trainer";
        internal static MelonPreferences_Entry<bool> AutoRespawn;

        internal static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory(SettingsCategory, "Duck Trainer");

            AutoRespawn = category.CreateEntry("AutoRespawn", false, "Auto Respawn Setting");
        }
    }
}