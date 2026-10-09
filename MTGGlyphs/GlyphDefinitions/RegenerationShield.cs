namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Regeneration's shield, as its reminder text puts it: "the next time this creature would be destroyed this turn, it isn't".</summary>
/// <exampledoc>Drudge Skeletons</exampledoc>
/// <examplecapture>the next time this creature would be destroyed this turn, it isn't</examplecapture>
public class RegenerationShield : Glyph
{
    public override Nib[] Nibs => ["the next time", Prop(Shielded), "would be destroyed this turn, it isn't"];

    public OneOf<ThisCreature, ThatCard> Shielded { get; set; }
}
