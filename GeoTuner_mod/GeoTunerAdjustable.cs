using HarmonyLib;
using KSerialization;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.UI.Image;

namespace GeoTuner_mod
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class GeoTunerAdjustable : KMonoBehaviour, IUserControlledCapacity
    {
   

        private static Options option = SingletonOptions<Options>.Instance;

        private static void OnCopySettings(GeoTunerAdjustable comp, object data)
        {
            comp.OnCopySettings(data);
        }

        public float UserMaxCapacity
        {
            get
            {
                return Math.Min(this.MAX_GEOTUNED, option.Maxcount) ;
            }
            set
            {
                this.MAX_GEOTUNED = value;
            }
        }

        public float AmountStored
        {
            get
            {
                return this.UserMaxCapacity;
            }
        }


        public float MinCapacity
        {
            get
            {
                return 1f;
            }
        }


        public float MaxCapacity
        {
            get
            {
                return option.Maxcount;
            }
        }


        public bool WholeValues
        {
            get
            {
                return false;
            }
        }

        bool IUserControlledCapacity.ControlEnabled()
		{
			return true;
		}
        public LocString CapacityUnits
        {
            get
            {
                return UI.UISIDESCREENS.GEOTUNERADJUSTABLE.UNITS;
            }
        }

        protected override void OnPrefabInit()
        {
         
            base.OnPrefabInit();

            base.Subscribe<GeoTunerAdjustable>(-905833192, GeoTunerAdjustable.OnCopySettingsDelegate);
        }


        protected override void OnSpawn()
        {
            this.Update();
        }

 
        internal void OnCopySettings(object data)
        {
            GeoTunerAdjustable component = ((GameObject)data).GetComponent<GeoTunerAdjustable>();
            bool flag = component != null;
            if (flag)
            {
                this.MAX_GEOTUNED = component.MAX_GEOTUNED;
            }
        }

    
        internal void Update()
        {

            bool flag = this.MAX_GEOTUNED != Old_GEOTUNED;

            if ( !flag)
            {
                return;
            }
            GeoTuner.Instance targetGeotuner = base.gameObject.GetSMI<GeoTuner.Instance>();

            if (targetGeotuner == null)
            {
                return;
            }
            Geyser assignedGeyser = targetGeotuner.GetFutureGeyser();


          
  
            if (flag && assignedGeyser)
            {
               StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.TargetParameter FutureGeyser = Traverse.Create(targetGeotuner.sm).Field("FutureGeyser").GetValue<StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.TargetParameter>();
                FutureGeyser.Set(null, targetGeotuner);
                targetGeotuner.AssignGeyser(null);
                targetGeotuner.AssignFutureGeyser(assignedGeyser);


                //global::Debug.Log("协调：" + this.MAX_GEOTUNED);
            }
            Old_GEOTUNED = this.MAX_GEOTUNED;

        }






        private static readonly EventSystem.IntraObjectHandler<GeoTunerAdjustable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<GeoTunerAdjustable>(new Action<GeoTunerAdjustable, object>(GeoTunerAdjustable.OnCopySettings));




        [MyCmpAdd]
        public CopyBuildingSettings copyBuildingSettings;



        [Serialize]
        private float Old_GEOTUNED = 1f;

        [MyCmpReq]
        public EnergyConsumer energyConsumer;

        [Serialize]
        private float MAX_GEOTUNED = 1f;
    }
}
