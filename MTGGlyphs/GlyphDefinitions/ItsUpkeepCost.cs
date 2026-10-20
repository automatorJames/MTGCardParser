namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"its upkeep cost", "its echo cost", "its entwine cost": a named cost the reminder refers back to.</summary>
/// <exampledoc>Energy Storm</exampledoc>
/// <examplecapture>its upkeep cost</examplecapture>
[Dependent]
public class ItsUpkeepCost : Glyph
{
    public override Nib[] Nibs => ["its", Prop(Kind), "cost"];

    public OneOf<UpkeepCostWord?, Keyword?> Kind { get; set; }
}
