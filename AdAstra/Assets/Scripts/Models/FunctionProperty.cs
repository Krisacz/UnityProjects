using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    public enum FunctionProperty
    {
        //=== STRUCTURE
        IsBlocking,
        Elevation,
        ConstructionTime,

        //==== RESOURCE
        Rarity,

        //==== MACHINE
        AssemblyTime,

        //==== TOOL
        ConstructionSpeed,
        DeconstructionSpeed,
        Reliability,
        Range,
        AssemblySpeed,
        DisassemblySpeed,
        RepairSpeed,
        RepairEfficiency,
        DrillSpeed,
        ExtractionYield,
        BurstExtractionChance,
        DrillHeadLevel,
        ScanSpeed,
        ScanSuccess,
        ScanLevel
    }
}
