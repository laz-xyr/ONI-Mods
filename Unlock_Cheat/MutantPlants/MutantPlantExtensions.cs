using System.Collections.Generic;
using UnityEngine;
using Klei.AI;
using System.Linq;
using HarmonyLib;
using EventSystem2Syntax;
using PeterHan.PLib.Options;
using System.Collections;
using System;
using static PlantSubSpeciesCatalog;

namespace Unlock_Cheat.MutantPlants
{
    internal static class MutantPlantExtensions
    {
        public static void DiscoverSilentlyAndIdentifySubSpecies(PlantSubSpeciesCatalog.SubSpeciesInfo speciesInfo)
        {
            List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(speciesInfo.speciesID);



            foreach (KeyValuePair<Tag, List<SubSpeciesInfo>> kvp in PlantSubSpeciesCatalog.Instance.discoveredSubspeciesBySpecies)
            {


                int count = kvp.Value.RemoveAll(e => e.mutationIDs.Contains("SelfHarvest"));
                Debug.LogFormat ("[测试] {0} 删除了：{1}" ,kvp.Key.Name, count);
            }

          int count1=  PlantSubSpeciesCatalog.Instance.identifiedSubSpecies.RemoveWhere(e => e.Name.Contains("SelfHarvest"));
            Debug.LogFormat("[测试] {0} 删除了：{1}", "identifiedSubSpecies", count1);


            if (allSubSpeciesForSpecies != null && !allSubSpeciesForSpecies.Contains(speciesInfo))
            {
                allSubSpeciesForSpecies.Add(speciesInfo);
                //if (speciesInfo.mutationIDs.Contains("SelfHarvest")) {

                //    HashSet<Tag> identifiedSubSpecies = Traverse.Create(PlantSubSpeciesCatalog.Instance).Field("identifiedSubSpecies").GetValue<HashSet<Tag>>();

                //    // Debug.Log("[测试] 前  "+ PlantSubSpeciesCatalog.Instance.identifiedSubSpecies.Count+"  "+ speciesInfo.ID);
                //    identifiedSubSpecies.Add(speciesInfo.ID);

                //    //Debug.Log("[测试] 后  " + PlantSubSpeciesCatalog.Instance.identifiedSubSpecies.Count + "  " + PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(speciesInfo.ID));

                //    foreach (MutantPlant mutantPlant in Components.MutantPlants)
                //    {
                //        if (mutantPlant.HasTag(speciesInfo.ID))
                //        {
                //            mutantPlant.UpdateNameAndTags();
                //        }
                //    }
                //    return;
                //}

            
                PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(speciesInfo.ID);
                SaveGame.Instance.ColonyAchievementTracker.LogAnalyzedSeed(speciesInfo.speciesID);
            }
        }

        internal static void Mutator(this MutantPlant mutant)
        {

           // List<string> strings = new List<string> { };
            if (mutant != null)
            {
                mutant.Mutate();

                mutant.ApplyMutator();

            }
        }


        internal static void ApplyMutator(this MutantPlant mutant,bool pop = true)
        {
            if (mutant != null)
            {



                if (mutant.MutationIDs != null && mutant.MutationIDs.Count > 0)
                {
                    KMonoBehaviour kMonoBehaviour = mutant;

                    if (kMonoBehaviour != null && pop)
                    {
                        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, Languages.UI.USERMENUACTIONS.HARVEST_WHEN_READY.Reload, kMonoBehaviour.transform, 3f, false);

                    }
                // mutant.delattr();


                }
               // mutant.SetSubSpecies(mutationIDs);
                mutant.ApplyMutations();
                mutant.AddTag(GameTags.MutatedSeed);
                if (mutant.HasTag(GameTags.Plant))
                {
                    MutantPlantExtensions.DiscoverSilentlyAndIdentifySubSpecies(mutant.GetSubSpeciesInfo());
                }
                else
                {
                    PlantSubSpeciesCatalog.Instance.DiscoverSubSpecies(mutant.GetSubSpeciesInfo(), mutant);
                }

                PlantBranchGrower.Instance smi = mutant.GetSMI<PlantBranchGrower.Instance>();
                if (!smi.IsNullOrStopped())
                {
                    smi.ActionPerBranch(delegate (GameObject go)
                    {
                        MutantPlant mutantPlant;
                        if (go.TryGetComponent<MutantPlant>(out mutantPlant))
                        {
                            mutant.CopyMutationsTo(mutantPlant);
                            mutantPlant.ApplyMutations();
                            MutantPlantExtensions.DiscoverSilentlyAndIdentifySubSpecies(mutantPlant.GetSubSpeciesInfo());
                        }
                    });
                }
                DetailsScreen.Instance.Trigger(-1514841199, null);

            }
        }

        internal static void IdentifyMutation(this MutantPlant mutant)
        {
            if (mutant != null)
            {
                mutant.Analyze();

                PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(mutant.SubSpeciesID);
                SaveGame.Instance.ColonyAchievementTracker.LogAnalyzedSeed(mutant.SpeciesID);

                DetailsScreen.Instance.Trigger(-1514841199, null);
            }
        }
    }

}
