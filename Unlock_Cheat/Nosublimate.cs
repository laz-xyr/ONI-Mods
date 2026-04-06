using HarmonyLib;
using System.Collections.Generic;

namespace Unlock_Cheat.Nosublimate
{


    [HarmonyPatch(typeof(Sublimates), "Sim200ms")]
    public class Sublimates_Sim200ms
    {
        private static List<SimHashes> Sublimates_Elements = null;

        public static bool Prefix(Sublimates __instance, float dt)
        {
            if (Sublimates_Elements == null) {

                Sublimates_Elements =  Unlock_Cheat.Options.GetSublimates();
            }
            SimHashes elementID = __instance.GetComponent<PrimaryElement>().ElementID;
            return !Sublimates_Elements.Contains(elementID);
        }
    }
}
