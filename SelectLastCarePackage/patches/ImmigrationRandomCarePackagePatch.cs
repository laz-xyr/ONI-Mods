using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace crazyxyr.SelectLastCarePackage.Patches
{
    [HarmonyPatch(typeof(Immigration), "RandomCarePackage")]
    public static class ImmigrationRandomCarePackagePatch // 随机补给包
    {
        public static bool Prefix(Immigration __instance, List<CarePackageInfo> ___carePackages, ref CarePackageInfo __result)
        {
            var context = SaveGame.Instance.GetComponent<ImmigrantScreenContext>();
            if ((___carePackages is null) || (context.Skip))

            {
                context.Skip = true;
                return true; 
            }
            var lastSelectedCarePackageInfo = context.LastSelectedCarePackageInfo;
            if (lastSelectedCarePackageInfo == null) return true;
            bool Find = ___carePackages.Any(p =>
                (p.requirement == null || p.requirement()) &&
                 p.id == lastSelectedCarePackageInfo.id);
            context.Skip = true;

            if (Find)
            {
                __result = lastSelectedCarePackageInfo;
                return false;
            }
            return true;

        }
    }
}