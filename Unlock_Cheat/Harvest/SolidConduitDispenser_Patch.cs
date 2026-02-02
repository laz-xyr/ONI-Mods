using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unlock_Cheat.MutantPlants.CopySetting;

namespace Unlock_Cheat.Harvest
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitDispenser_Patch : KMonoBehaviour, ISaveLoadable, IConduitDispenser
    {

        public Storage Storage
        {
            get
            {
                return this.storage;
            }
        }


        public ConduitType ConduitType
        {
            get
            {
                return ConduitType.Solid;
            }
        }


        public SolidConduitFlow.ConduitContents ConduitContents
        {
            get
            {
                return this.GetConduitFlow().GetContents(this.utilityCell);
            }
        }


        public bool IsDispensing
        {
            get
            {
                return this.dispensing;
            }
        }

        public SolidConduitFlow GetConduitFlow()
        {
            return Game.Instance.solidConduitFlow;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.utilityCell = this.GetOutputCell();
            ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
            this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
            this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
            this.OnConduitConnectionChanged(null);
        }

        public override void OnCleanUp()
        {
            this.GetConduitFlow().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
            GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
            base.OnCleanUp();
        }

        private void OnConduitConnectionChanged(object data)
        {
            this.dispensing = (this.dispensing && this.IsConnected);
            base.Trigger(-2094018600, BoxedBools.Box(this.IsConnected));
        }

        private void ConduitUpdate(float dt)
        {
            bool flag = false;
            this.operational.SetFlag(SolidConduitDispenser_Patch.outputConduitFlag, this.IsConnected);
            if (this.operational.IsOperational || this.alwaysDispense)
            {
                SolidConduitFlow conduitFlow = this.GetConduitFlow();
                if (conduitFlow.HasConduit(this.utilityCell) && conduitFlow.IsConduitEmpty(this.utilityCell))
                {
                    Pickupable pickupable = this.FindSuitableItem();
                    if (pickupable)
                    {
                        //if (pickupable.PrimaryElement.Mass > 20f)
                        //{
                        //    pickupable = pickupable.Take(Mathf.Max(20f, pickupable.PrimaryElement.MassPerUnit));
                        //}
                        conduitFlow.AddPickupable(this.utilityCell, pickupable);
                        flag = true;
                    }
                }
            }
            this.storage.storageNetworkID = this.GetConnectedNetworkID();
            this.dispensing = flag;
        }

        private bool isSolid(GameObject o)
        {
            PrimaryElement component = o.GetComponent<PrimaryElement>();
            return !(component == null) && (component.Element.IsSolid || (double)component.MassPerUnit != 1.0);
        }

        private Pickupable FindSuitableItem()
        {
            List<GameObject> items = this.storage.items;
            if (items.Count < 1)
            {
                return null;
            }
            this.round_robin_index %= items.Count;
            GameObject gameObject = items[this.round_robin_index];
            this.round_robin_index++;
            if (this.solidOnly && !this.isSolid(gameObject))
            {
                bool flag = false;
                int num = 0;
                while (!flag && num < items.Count)
                {
                    gameObject = items[(this.round_robin_index + num) % items.Count];
                    if (this.isSolid(gameObject))
                    {
                        flag = true;
                    }
                    num++;
                }
                if (!flag)
                {
                    return null;
                }
            }
            if (!gameObject)
            {
                return null;
            }
            return gameObject.GetComponent<Pickupable>();
        }


        public bool IsConnected
        {
            get
            {
                GameObject gameObject = Grid.Objects[this.utilityCell, 20];
                return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
            }
        }


        private int GetConnectedNetworkID()
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, 20];
            SolidConduit solidConduit = (gameObject != null) ? gameObject.GetComponent<SolidConduit>() : null;
            UtilityNetwork utilityNetwork = (solidConduit != null) ? solidConduit.GetNetwork() : null;
            if (utilityNetwork == null)
            {
                return -1;
            }
            return utilityNetwork.id;
        }


        private int GetOutputCell()
        {
            Building component = base.GetComponent<Building>();
            if (this.useSecondaryOutput)
            {
                foreach (ISecondaryOutput secondaryOutput in base.GetComponents<ISecondaryOutput>())
                {
                    if (secondaryOutput.HasSecondaryConduitType(ConduitType.Solid))
                    {
                        return Grid.OffsetCell(component.NaturalBuildingCell(), secondaryOutput.GetSecondaryConduitOffset(ConduitType.Solid));
                    }
                }
                return Grid.OffsetCell(component.NaturalBuildingCell(), CellOffset.none);
            }
            return component.GetUtilityOutputCell();
        }

        [SerializeField]
        public SimHashes[] elementFilter;

        [SerializeField]
        public bool invertElementFilter;

        [SerializeField]
        public bool alwaysDispense;

        [SerializeField]
        public bool useSecondaryOutput;

        [SerializeField]
        public bool solidOnly;

        private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

        [MyCmpReq]
#pragma warning disable CS0649 // 禁用 "从未赋值" 警告
        private Operational operational;
#pragma warning restore CS0649 // 恢复警告

        [MyCmpReq]
        public Storage storage;

        private HandleVector<int>.Handle partitionerEntry;

        private int utilityCell = -1;

        private bool dispensing;

        private int round_robin_index;
    }

}
