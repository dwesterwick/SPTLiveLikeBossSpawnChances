using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DansDevTools.Patches
{
    internal class BotsControllerSetSettingsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotsController).GetMethod(nameof(BotsController.SetSettings), BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        protected static void PatchPostfix()
        {
            Singleton<GameWorld>.Instance.gameObject.GetOrAddComponent<Components.CheatsComponent>();
        }
    }
}
