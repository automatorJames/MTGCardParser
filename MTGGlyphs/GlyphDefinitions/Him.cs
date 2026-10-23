namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"him" / "her": a legend or planeswalker referred back to by its personal pronoun, as counters' holder ("captain america has a shield counter on him").</summary>
/// <exampledoc>Captain America, Super-Soldier</exampledoc>
/// <examplecapture>him</examplecapture>
[Dependent]
public class Him : BackReference
{
    public override Nib[] Nibs => [Alt("him", "her")];
}
