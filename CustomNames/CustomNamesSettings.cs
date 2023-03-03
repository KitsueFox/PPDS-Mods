using MelonLoader;

#nullable disable

namespace Custom_Names
{
    public static class CustomNameSettings //MelonSettings for the Mod
    {
        private const string SettingsCategory = "CustomNames";
        internal static MelonPreferences_Entry<bool> AutoName;

        internal static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory(SettingsCategory, "CustomNames");
            AutoName = category.CreateEntry("AutoName", false, "Auto Name",
                "Newly Spawning Ducks will be Auto Nammed via clipboard");
        }
    }
}