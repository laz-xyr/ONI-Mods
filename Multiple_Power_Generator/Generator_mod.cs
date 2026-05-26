using Epic.OnlineServices.Platform;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiple_Power_Generator
{

    internal class HarmonyPatches : UserMod2
    {
        // Token: 0x0600002A RID: 42 RVA: 0x0000239F File Offset: 0x0000059F
        public override void OnLoad(Harmony harmony)
        {

            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Options));
            base.OnLoad(harmony);

        }
    }

        internal class Patches
    {
        

        [HarmonyPatch(typeof(Generator), "WattageRating", MethodType.Getter)]
        public class Generator_WattageRating
        {
            // Token: 0x060007B0 RID: 1968 RVA: 0x0001C852 File Offset: 0x0001AA52
            private static void Postfix(ref float __result)
            {
                __result *= SingletonOptions<Options>.Instance.PowerRatio;
            }
        }

        [HarmonyPatch(typeof(Generator), "BaseWattageRating", MethodType.Getter)]
        public class Generator_BaseWattageRating
        {
            // Token: 0x060007B2 RID: 1970 RVA: 0x0001C86B File Offset: 0x0001AA6B
            private static void Postfix(ref float __result)
            {
                __result *= SingletonOptions<Options>.Instance.PowerRatio;
            }
        }

        // Token: 0x020000E7 RID: 231
        [HarmonyPatch(typeof(Generator), "CalculateCapacity")]
        public class Generator_CalculateCapacity
        {
            // Token: 0x060007B4 RID: 1972 RVA: 0x0001C884 File Offset: 0x0001AA84
            private static void Postfix(ref float __result)
            {
                __result *= SingletonOptions<Options>.Instance.PowerRatio;
            }
        }

        [HarmonyPatch(typeof(Wire), "GetMaxWattageAsFloat")]
        public class Wire_GetMaxWattageAsFloat
        {
            // Token: 0x060007B6 RID: 1974 RVA: 0x0001C89D File Offset: 0x0001AA9D
            private static void Postfix(ref float __result)
            {
                __result *= SingletonOptions<Options>.Instance.WireRatio;
            }
        }



        [HarmonyPatch(typeof(BatteryMediumConfig), "DoPostConfigureComplete")]
        public class Battery_BatteryMediumConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(BatteryMediumConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }

        [HarmonyPatch(typeof(BatteryConfig), "DoPostConfigureComplete")]
        public class Battery_BatteryConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(BatteryConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }
        [HarmonyPatch(typeof(BatteryModuleConfig), "DoPostConfigureComplete")]
        public class Battery_BatteryModuleConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(BatteryModuleConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }
        [HarmonyPatch(typeof(BatterySmartConfig), "DoPostConfigureComplete")]
        public class Battery_BatterySmartConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(BatterySmartConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }
        [HarmonyPatch(typeof(PowerTransformerConfig), "DoPostConfigureComplete")]
        public class Battery_PowerTransformerConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(PowerTransformerConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }
        [HarmonyPatch(typeof(PowerTransformerSmallConfig), "DoPostConfigureComplete")]
        public class Battery_PowerTransformerSmallConfig
        {
            // Token: 0x060007A8 RID: 1960 RVA: 0x0001C710 File Offset: 0x0001A910
            public static void Postfix(PowerTransformerSmallConfig __instance, GameObject go)
            {
                Battery battery = go.AddOrGet<Battery>();
                battery.capacity = battery.capacity * SingletonOptions<Options>.Instance.BatteryRatio;


            }
        }
    }
}
