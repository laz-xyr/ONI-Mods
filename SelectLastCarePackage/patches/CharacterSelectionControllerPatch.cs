using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarePackageContainer;

namespace crazyxyr.SelectLastCarePackage.Patches2
{

    [HarmonyPatch(typeof(CharacterSelectionController), "AddDeliverable")]
    public class CharacterSelectionControllerPatch
    {


        public static bool Prefix(CharacterSelectionController __instance)
        {

            List<ITelepadDeliverable> selectedDeliverables = Traverse.Create(__instance).Field("selectedDeliverables").GetValue<List<ITelepadDeliverable>>();
           int  selectableCount = Traverse.Create(__instance).Field("selectableCount").GetValue <int>();
            if (selectedDeliverables.Count >= selectableCount)
            {

                ITelepadDeliverable del = selectedDeliverables[selectedDeliverables.Count -1];
                __instance.RemoveDeliverable(del);
                if (del is CarePackageInstanceData carePackageContainer) {

                    global::Debug.Log("处理补给包错误: " + carePackageContainer.info.id);
                }
                else global::Debug.Log("处理补给包多选错误");
            }

            return true;

        }

    }

    [HarmonyPatch(typeof(CharacterContainer), "Reshuffle")]
    public class CharacterContainerPatch
    {
        public static void Prefix(CharacterContainer __instance, CharacterSelectionController ___controller, ref bool is_starter)
        {
            if (___controller != null )
            {
                ___controller.RemoveLast();
            }
            is_starter = ___controller.IsStarterMinion;
        }

    }

}
