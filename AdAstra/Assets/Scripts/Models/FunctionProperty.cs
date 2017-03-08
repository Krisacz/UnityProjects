namespace Assets.Scripts.Models
{
    public enum FunctionProperty
    {
        //=== STRUCTURE
        IsBlocking,
        Elevation,
        ConstructionTime,   //same time for deconstruction

        //==== RESOURCE
        OreRarity,
        ScanLevelRequired,
        OreDensity,
        OreProcessTime,

        //==== MACHINE
        WorkSpeed,
        AssemblyTime,
        InteractUIType,

        //==== TOOL
        ConstructionSpeed,
        DeconstructionSpeed,
        Reliability,
        Range,
        AssemblySpeed,
        DisassemblySpeed,
        RepairSpeed,
        RepairEfficiencyMin,
        RepairEfficiencyMax,
        DrillSpeed,
        ExtractionYieldMin,
        ExtractionYieldMax,
        BurstExtractionChance,
        DrillHeadLevel,
        ScanSpeed,
        ScanSuccess,
        ScanLevel
    }
}
