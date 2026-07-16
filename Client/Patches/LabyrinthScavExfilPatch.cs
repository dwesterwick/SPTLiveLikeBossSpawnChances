using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DansDevTools.Utils;
using SPT.Reflection.Patching;
using UnityEngine;

namespace DansDevTools.Patches
{
    internal class LabyrinthScavExfilPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExfiltrationControllerClass).GetMethod
            (
                nameof(ExfiltrationControllerClass.ScavExfiltrationClaim),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[] { typeof(Vector3), typeof(string), typeof(int) },
                null
            );
        }

        [PatchPrefix]
        protected static bool PatchPrefix(ExfiltrationControllerClass __instance)
        {
            if (__instance.ScavExfiltrationPoints.Length == 0)
            {
                Comfort.Common.Singleton<LoggingUtil>.Instance.LogInfo("No Scav exfil points found; skipping " + nameof(ExfiltrationControllerClass.ScavExfiltrationClaim));
                return false;
            }

            return true;
        }
    }
}
