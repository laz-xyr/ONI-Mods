using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Unlock_Cheat.Harvest
{
    internal class Harvest
    {
        [HarmonyPatch(typeof(NoseconeHarvestConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public class harvestModule
        {
            public static void Postfix(NoseconeHarvestConfig __instance, GameObject go)
            {
                go.AddOrGetDef<ResourceHarvestModule.Def>().harvestSpeed = __instance.solidCapacity / __instance.timeToFill * Unlock_Cheat.Options.Harvest_mult;
            }
        }

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance))]
        public class ResourceHarvestModule_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("GetMaxExtractKGFromDiamondAvailable")]

            public static void Postfix1(ref float __result)
            {
                __result *= Unlock_Cheat.Options.Harvest_mult;
            }

            [HarmonyPrefix]
            [HarmonyPatch("ConsumeDiamond")]

            public static void Prefix1(ref float amount)
            {
                amount /= Unlock_Cheat.Options.Harvest_mult;
            }
        }


        [HarmonyPatch(typeof(HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration))]
        [HarmonyPatch("Init")]
        public static class HarvestablePOIInstanceConfiguration_Init
        {
            static   bool    state;
            public static void Prefix(HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration __instance,bool __state, bool ___didInit)
            {
                state = ___didInit;
            }
            public static void Postfix(HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration __instance, bool __state, bool ___didInit)
            {
                if (state || state == ___didInit)
                {
                    return;
                }
                __instance.poiTotalCapacity *= Unlock_Cheat.Options.Harvest_poi_mult;
            }
        }

        [HarmonyPatch(typeof(BuildingTemplates))]
        [HarmonyPatch("ExtendBuildingToClusterCargoBay", new Type[] { typeof(GameObject), typeof(float), typeof(List<Tag>), typeof(List<Tag>), typeof(CargoBay.CargoType) })]
        public class BuildingTemplates_ExtendBuildingToClusterCargoBay
        {
            public static void Prefix(ref float capacity)
            {
                capacity *= Unlock_Cheat.Options.Harvest_storage_mult;
            }
        }


        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]

        public class BaseModularLaunchpadPortConfig_ConfigureBuildingTemplate
        {
            public static void Prefix(ref float storageSize)
            {
                storageSize *= Unlock_Cheat.Options.Harvest_storage_mult * 1000;
            }
        }
    }
}
