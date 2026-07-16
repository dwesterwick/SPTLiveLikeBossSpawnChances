using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DansDevTools.Patches
{
    internal class IsDayByHourPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ZoneLeaveControllerClass).GetMethod(nameof(ZoneLeaveControllerClass.IsDayByHour), BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        protected static bool PatchPrefix(bool __result)
        {
            if (!DansDevToolsPlugin.DayTimeCultists.Value)
            {
                return true;
            }

            __result = false;
            return false;
        }
    }
}
