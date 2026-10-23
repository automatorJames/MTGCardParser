namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Abilities a copy gains: "except it has haste", "except it has this ability" (the copying ability itself, so the copy can copy again), "except it has flying and this ability", "except he has hexproof".</summary>
/// <exampledoc>Cryptoplasm</exampledoc>
/// <examplecapture>except it has this ability</examplecapture>
[Dependent]
public class CopyHasAbility : Glyph
{
    public override Nib[] Nibs => ["except", Prop(Holder), Prop(Keywords)];

    public CopyPronoun Holder { get; set; }
    public CopyHasKeywords Keywords { get; set; }
}
