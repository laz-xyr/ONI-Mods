using HarmonyLib;
using System;
using UnityEngine;
using Klei.AI;
using Database;
using PeterHan.PLib.Options;
using static Unlock_Cheat.Languages.UI.USERMENUACTIONS;
using System.Linq;
using static DiscreteShadowCaster;

namespace Unlock_Cheat.MutantPlants
{
    internal class MutantPlantPatches
    {

        private static void OnRefreshUserMenu(MutantPlant mutant)
        {
            KPrefabID kprefabID;
            if (mutant != null && mutant.TryGetComponent<KPrefabID>(out kprefabID))
            {

                if ( (mutant.IsOriginal && !kprefabID.HasTag(GameTags.PlantBranch)) || kprefabID.HasTag(GameTags.Seed) || kprefabID.HasTag(GameTags.CropSeed) || 
                    (kprefabID.HasTag(GameTags.MutatedSeed)  && (SingletonOptions<Options>.Instance.MutantPlant_Mult  )))
                {
                    KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_select_research", Languages.UI.USERMENUACTIONS.MUTATOR.NAME, new System.Action(mutant.Mutator), global::Action.NumActions, null, null, null, Languages.UI.USERMENUACTIONS.MUTATOR.TOOLTIP, true);
                    Game.Instance.userMenu.AddButton(mutant.gameObject, button, 1f);

                    }

                    if (!mutant.IsOriginal && !mutant.IsIdentified)
                {
                        KIconButtonMenu.ButtonInfo button2 = new KIconButtonMenu.ButtonInfo("action_select_research", Languages.UI.USERMENUACTIONS.IDENTIFY_MUTATION.NAME, new System.Action(mutant.IdentifyMutation), global::Action.NumActions, null, null, null, Languages.UI.USERMENUACTIONS.IDENTIFY_MUTATION.TOOLTIP, true);
                    Game.Instance.userMenu.AddButton(mutant.gameObject, button2, 1f);
                }
                }
            }
       
        private static void OnCopySettings(MutantPlant newdata,object data)
        {
            GameObject gameObject = (GameObject)data;
            bool flag = !(gameObject == null);
            if (flag)
            {
                MutantPlant component = gameObject.GetComponent<MutantPlant>();
                bool flag2 = !(component == null);             
                if (flag2)
                {
                    //newdata.SetSubSpecies(component.MutationIDs);
                    //newdata.ApplyMutator();
                    component.CopyMutationsTo(newdata);
                    newdata.ApplyMutations();
                    MutantPlantExtensions.DiscoverSilentlyAndIdentifySubSpecies(newdata.GetSubSpeciesInfo());

                }
            }
        }

        private static readonly EventSystem.IntraObjectHandler<MutantPlant> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<MutantPlant>(delegate (MutantPlant component, object data)
        {
            MutantPlantPatches.OnRefreshUserMenu(component);
        });

        private static readonly EventSystem.IntraObjectHandler<MutantPlant> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<MutantPlant>(delegate (MutantPlant component, object data)
        {
            MutantPlantPatches.OnCopySettings(component,data);
        });

        [HarmonyPatch(typeof(MutantPlant), "OnSpawn")]
        public static class MutantPlant_OnSpawn
        {
            public static void Postfix(MutantPlant __instance)
            {
                __instance.Subscribe<MutantPlant>(493375141, MutantPlantPatches.OnRefreshUserMenuDelegate);
                __instance.Subscribe<MutantPlant>(-905833192, MutantPlantPatches.OnCopySettingsDelegate);
            }
        }

        [HarmonyPatch(typeof(MutantPlant), "OnCleanUp")]
        public static class MutantPlant_OnCleanUp
        {
            public static void Prefix(MutantPlant __instance)
            {
                __instance.Unsubscribe<MutantPlant>(493375141, MutantPlantPatches.OnRefreshUserMenuDelegate, false);
                __instance.Unsubscribe<MutantPlant>(-905833192, MutantPlantPatches.OnCopySettingsDelegate, false);

            }
        }



        [HarmonyPatch(typeof(PlantMutation), "ApplyFunctionalTo")]
        public static class PlantMutation_ApplyFunctionalTo
        {
            public static void Postfix(PlantMutation __instance, MutantPlant target)
            {

                SeedProducer component = target.GetComponent<SeedProducer>();

                if (component != null && component.seedInfo.productionType == SeedProducer.ProductionType.Sterile)
                {
                    component.Configure(component.seedInfo.seedId, SeedProducer.ProductionType.Harvest, 1);
                }

            }
        }


        [HarmonyPatch(typeof(PlantMutation), "AttributeModifier")]
        public static class PlantMutation_AttributeModifier
        {
            public static void Prefix(PlantMutation __instance, Klei.AI.Attribute attribute, ref float amount)
            {
                if (attribute == Db.Get().PlantAttributes.MinRadiationThreshold || attribute == Db.Get().PlantAttributes.MinLightLux)
                {

                    amount = 0;
                }
            }
        }


    }
}
