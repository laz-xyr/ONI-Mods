using HarmonyLib;
using Klei;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000872 RID: 2162
[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDispenser_Patch: KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
    // Token: 0x17000412 RID: 1042
    // (get) Token: 0x06003B5C RID: 15196 RVA: 0x0014BD47 File Offset: 0x00149F47
    public Storage Storage
    {
        get
        {
            return this.storage;
        }
    }

    // Token: 0x17000413 RID: 1043
    // (get) Token: 0x06003B5D RID: 15197 RVA: 0x0014BD4F File Offset: 0x00149F4F
    public ConduitType ConduitType
    {
        get
        {
            return this.conduitType;
        }
    }

    // Token: 0x17000414 RID: 1044
    // (get) Token: 0x06003B5E RID: 15198 RVA: 0x0014BD57 File Offset: 0x00149F57
    public ConduitFlow.ConduitContents ConduitContents
    {
        get
        {
            return this.GetConduitManager().GetContents(this.utilityCell);
        }
    }

    // Token: 0x06003B5F RID: 15199 RVA: 0x0014BD6A File Offset: 0x00149F6A
    public void SetConduitData(ConduitType type)
    {
        this.conduitType = type;
    }

    // Token: 0x06003B60 RID: 15200 RVA: 0x0014BD74 File Offset: 0x00149F74
    public ConduitFlow GetConduitManager()
    {
        ConduitType conduitType = this.conduitType;
        if (conduitType == ConduitType.Gas)
        {
            return Game.Instance.gasConduitFlow;
        }
        if (conduitType != ConduitType.Liquid)
        {
            return null;
        }
        return Game.Instance.liquidConduitFlow;
    }

    // Token: 0x06003B61 RID: 15201 RVA: 0x0014BDA9 File Offset: 0x00149FA9
    private void OnConduitConnectionChanged(object data)
    {
        base.Trigger(-2094018600, BoxedBools.Box(this.IsConnected));
    }

    // Token: 0x06003B62 RID: 15202 RVA: 0x0014BDC4 File Offset: 0x00149FC4
    public override void OnSpawn()
    {
        base.OnSpawn();
        GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate (object obj)
        {
            Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing, true);
        }, null, null);
        ConduitFlow conduitManager = this.GetConduitManager();
        this.utilityCell = this.GetOutputCell(conduitManager.conduitType);
        ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
        this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
        this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
        this.OnConduitConnectionChanged(null);
    }

    // Token: 0x06003B63 RID: 15203 RVA: 0x0014BE8F File Offset: 0x0014A08F
    public override void OnCleanUp()
    {
        this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
        GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
        base.OnCleanUp();
    }

    // Token: 0x06003B64 RID: 15204 RVA: 0x0014BEBE File Offset: 0x0014A0BE
    public void SetOnState(bool onState)
    {
        this.isOn = onState;
    }

    // Token: 0x06003B65 RID: 15205 RVA: 0x0014BEC7 File Offset: 0x0014A0C7
    private void ConduitUpdate(float dt)
    {
        if (this.operational != null)
        {
            this.operational.SetFlag(ConduitDispenser_Patch.outputConduitFlag, this.IsConnected);
        }
        this.blocked = false;
        if (this.isOn)
        {
            this.Dispense(dt);
        }
    }

    public float AddElement(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
    {
        ConduitFlow conduitflow = this.GetConduitManager();
        ConduitFlow.GridNode[] grid = Traverse.Create(conduitflow).Field("grid").GetValue<ConduitFlow.GridNode[]>();

        if (grid[cell_idx].conduitIdx == -1)
        {
            return 0f;
        }

        ConduitFlow.ConduitContents contents = conduitflow.GetConduit(cell_idx).GetContents(conduitflow);
        if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
        {
            return 0f;
        }

        float num = Mathf.Min(mass, float.PositiveInfinity - contents.mass);
        float num2 = num / mass;
        if (num <= 0f)
        {
            return 0f;
        }

        contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
        contents.AddMass(num);
        contents.element = element;
        contents.ConsolidateMass();
        int num3 = (int)(num2 * (float)disease_count);
        if (num3 > 0)
        {
            SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
            contents.diseaseIdx = diseaseInfo.idx;
            contents.diseaseCount = diseaseInfo.count;
        }

        conduitflow.SetContents(cell_idx, contents);
        return num;
    }
    private void Dispense(float dt)
    {
        if ((this.operational != null && this.operational.IsOperational) || this.alwaysDispense)
        {
            if (this.building != null && this.building.Def.CanMove)
            {
                this.utilityCell = this.GetOutputCell(this.GetConduitManager().conduitType);
            }
            PrimaryElement primaryElement = this.FindSuitableElement();
            if (primaryElement != null)
            {
                primaryElement.KeepZeroMassObject = true;
                this.empty = false;
                float num = this.GetConduitManager().AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                if (num > 0f)
                {
                    int num2 = (int)(num / primaryElement.Mass * (float)primaryElement.DiseaseCount);
                    primaryElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
                    primaryElement.Mass -= num;
                    this.storage.Trigger(-1697596308, primaryElement.gameObject);
                    return;
                }
                this.blocked = true;
                return;
            }
            else
            {
                this.empty = true;
            }
        }
    }

    // Token: 0x06003B67 RID: 15207 RVA: 0x0014C01C File Offset: 0x0014A21C
    private PrimaryElement FindSuitableElement()
    {
        List<GameObject> items = this.storage.items;
        int count = items.Count;
        for (int i = 0; i < count; i++)
        {
            int index = (i + this.elementOutputOffset) % count;
            PrimaryElement component = items[index].GetComponent<PrimaryElement>();
            if (component != null && component.Mass > 0f && ((this.conduitType == ConduitType.Liquid) ? component.Element.IsLiquid : component.Element.IsGas) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
            {
                this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                return component;
            }
        }
        return null;
    }

    // Token: 0x06003B68 RID: 15208 RVA: 0x0014C0FC File Offset: 0x0014A2FC
    private bool IsFilteredElement(SimHashes element)
    {
        for (int num = 0; num != this.elementFilter.Length; num++)
        {
            if (this.elementFilter[num] == element)
            {
                return true;
            }
        }
        return false;
    }

    // Token: 0x17000415 RID: 1045
    // (get) Token: 0x06003B69 RID: 15209 RVA: 0x0014C12C File Offset: 0x0014A32C
    public bool IsConnected
    {
        get
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16];
            return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
        }
    }

    // Token: 0x06003B6A RID: 15210 RVA: 0x0014C170 File Offset: 0x0014A370
    private int GetOutputCell(ConduitType outputConduitType)
    {
        Building component = base.GetComponent<Building>();
        if (!(component != null))
        {
            return Grid.OffsetCell(Grid.PosToCell(this), this.noBuildingOutputCellOffset);
        }
        if (this.useSecondaryOutput)
        {
            ISecondaryOutput[] components = base.GetComponents<ISecondaryOutput>();
            foreach (ISecondaryOutput secondaryOutput in components)
            {
                if (secondaryOutput.HasSecondaryConduitType(outputConduitType))
                {
                    return Grid.OffsetCell(component.NaturalBuildingCell(), secondaryOutput.GetSecondaryConduitOffset(outputConduitType));
                }
            }
            return Grid.OffsetCell(component.NaturalBuildingCell(), components[0].GetSecondaryConduitOffset(outputConduitType));
        }
        return component.GetUtilityOutputCell();
    }

    // Token: 0x040024B6 RID: 9398
    [SerializeField]
    public ConduitType conduitType;

    // Token: 0x040024B7 RID: 9399
    [SerializeField]
    public SimHashes[] elementFilter;

    // Token: 0x040024B8 RID: 9400
    [SerializeField]
    public bool invertElementFilter;

    // Token: 0x040024B9 RID: 9401
    [SerializeField]
    public bool alwaysDispense;

    // Token: 0x040024BA RID: 9402
    [SerializeField]
    public bool isOn = true;

    // Token: 0x040024BB RID: 9403
    [SerializeField]
    public bool blocked;

    // Token: 0x040024BC RID: 9404
    [SerializeField]
    public bool empty = true;

    // Token: 0x040024BD RID: 9405
    [SerializeField]
    public bool useSecondaryOutput;

    // Token: 0x040024BE RID: 9406
    [SerializeField]
    public CellOffset noBuildingOutputCellOffset;

    // Token: 0x040024BF RID: 9407
    private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

    // Token: 0x040024C0 RID: 9408
    [MyCmpGet]
    private Operational operational;

    // Token: 0x040024C1 RID: 9409
    [MyCmpReq]
    public Storage storage;

    // Token: 0x040024C2 RID: 9410
    [MyCmpGet]
    private Building building;

    // Token: 0x040024C3 RID: 9411
    private HandleVector<int>.Handle partitionerEntry;

    // Token: 0x040024C4 RID: 9412
    private int utilityCell = -1;

    // Token: 0x040024C5 RID: 9413
    private int elementOutputOffset;
}
