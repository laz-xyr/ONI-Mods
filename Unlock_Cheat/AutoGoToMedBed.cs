using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unlock_Cheat.AutoGoToMedBed
{
    internal class AutoGoToMedBed
    {
        [HarmonyPatch(typeof(WoundMonitor))]
        [HarmonyPatch("InitializeStates")]
        public class WoundMonitorInitializeStates
        {
            // Token: 0x06000021 RID: 33 RVA: 0x000028B4 File Offset: 0x00000AB4
            public static void Postfix(ref WoundMonitor __instance)
            {
                __instance.wounded.light.ToggleUrge(Db.Get().Urges.Heal).Update("AutoAssignClinic", delegate (WoundMonitor.Instance smi, float dt)
                {
                    smi.FindAvailableMedicalBed();
                }, UpdateRate.SIM_1000ms, false);
                __instance.wounded.medium.ToggleUrge(Db.Get().Urges.Heal).Update("AutoAssignClinic", delegate (WoundMonitor.Instance smi, float dt)
                {
                    smi.FindAvailableMedicalBed();

                }, UpdateRate.SIM_1000ms, false);

            }

            // Token: 0x06000022 RID: 34 RVA: 0x000029CC File Offset: 0x00000BCC
            //public static void AutoAssignClinic(WoundMonitor.Instance smi)
            //{
            //    Ownables soleOwner = smi.gameObject.GetComponent<MinionIdentity>().GetSoleOwner();
            //    AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
            //    AssignableSlotInstance slot = soleOwner.GetSlot(clinic);
            //    if (slot == null || slot.assignable != null)
            //    {
            //        return;
            //    }
            //    soleOwner.AutoAssignSlot(clinic);
            //}


        }
    }
}
