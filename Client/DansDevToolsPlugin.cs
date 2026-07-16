using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using DansDevTools.Utils;
using DansDevTools.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansDevTools
{
    [BepInPlugin(ModInfo.GUID, ModInfo.MODNAME, ModInfo.MOD_VERSION)]
    internal class DansDevToolsPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> GodMode = null!;
        public static ConfigEntry<bool> InfiniteStamina = null!;
        public static ConfigEntry<bool> InfiniteHydrationAndEnergy = null!;
        public static ConfigEntry<bool> DayTimeCultists = null!;

        protected void Awake()
        {
            Logger.LogInfo("Loading DansDevTools...");

            Singleton<LoggingUtil>.Create(new LoggingUtil(Logger));

            if (ConfigUtil.CurrentConfig.Enabled)
            {
                Singleton<LoggingUtil>.Instance.LogInfo("Loading DansDevTools...enabled");

                new BotsControllerSetSettingsPatch().Enable();
                new GodModePatch().Enable();
                new IsDayByHourPatch().Enable();

                if (ConfigUtil.CurrentConfig.FreeLabyrinthAccess)
                {
                    new LabyrinthScavExfilPatch().Enable();
                }

                AddConfigOptions(Config);
            }

            Singleton<LoggingUtil>.Instance.LogInfo("Loading DansDevTools...done.");
        }

        private void AddConfigOptions(ConfigFile Config)
        {
            GodMode = Config.Bind("Cheats", "God Mode", false, "Ignores all damage");
            InfiniteStamina = Config.Bind("Cheats", "Infinite Stamina", false, "Infinite stamina");
            InfiniteHydrationAndEnergy = Config.Bind("Cheats", "Infinite Hydration and Energy", false, "No hydration or energy loss during raids");
            DayTimeCultists = Config.Bind("Gameplay Changes", "Daytime Cultists", false, "Allows Cultists to spawn during the day");
        }
    }
}
