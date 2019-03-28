﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using SRML.SR.SaveSystem.Data.Ammo;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SRML.SR.SaveSystem.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("GetSelectedStored")]
    internal static class AmmoGetSelectedStoredPatch
    {
        public static void Postfix(Ammo __instance, GameObject __result)
        {
            if (!__result) return;

            if((new StackFrame(2)).GetMethod().DeclaringType==typeof(VacColorAnimator)) return;

            var identifier = AmmoIdentifier.GetIdentifier(__instance);
            if (identifier.AmmoType != AmmoType.NONE)
            {
                if (PersistentAmmoManager.HasPersistentAmmo(identifier))
                {
                    PersistentAmmoManager.PersistentAmmoData[identifier].OnSelected(Identifiable.GetId(__result),__instance.selectedAmmoIdx);
                }
            }
        }
    }
}
