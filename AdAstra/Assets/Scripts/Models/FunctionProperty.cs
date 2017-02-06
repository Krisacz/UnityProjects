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
        ConstructionTime,   //same time for deconstruction

        //==== RESOURCE
        Rarity,

        //==== MACHINE
        AssemblyTime,       //same time for disassembly

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
