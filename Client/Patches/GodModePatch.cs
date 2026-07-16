using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DansDevTools.Patches
{
    internal class GodModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ActiveHealthController).GetMethod(nameof(ActiveHealthController.ApplyDamage), BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        protected static bool PatchPrefix(ActiveHealthController __instance, ref float damage)
        {
            if (!DansDevToolsPlugin.GodMode.Value)
            {
                return true;
            }

            if (Singleton<GameWorld>.Instance.MainPlayer != __instance.Player)
            {
                return true;
            }

            damage = 0f;
            return false;
        }
    }
}
