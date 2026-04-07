using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STRINGS;
using Database;
using Klei.AI;

namespace Unlock_Cheat.MutantPlants.SelfHarvestPatch
{
    [HarmonyPatch(typeof(PlantMutations))]
    public static class PlantMutation_PlantMutations
    {

        [HarmonyPostfix]
        [HarmonyPatch("AddPlantMutation")]
        public static void Postfix(PlantMutation __result)
        {
            __result.ForceSelfHarvestOnGrown();
        }


        //[HarmonyPostfix]
        //[HarmonyPatch(MethodType.Constructor, new Type[] { typeof(ResourceSet) })]
        //public static void Postfix1(PlantMutations __instance)
        //{

        //    PlantMutation plantMutation = new PlantMutation("SelfHarvest", Languages.UI.USERMENUACTIONS.SELFHARVEST.MutationNAME, Languages.UI.USERMENUACTIONS.SELFHARVEST.TOOLTIP);
        //    plantMutation.ForceSelfHarvestOnGrown();
        //    //  plantMutation.originalMutation = true;
        //    __instance.Add(plantMutation);

        //}

    }



}
