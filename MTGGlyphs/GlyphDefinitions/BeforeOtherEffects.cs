namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect done first, ahead of the rest of the spell: "they draw a card before its other effects" (a promised gift).</summary>
/// <exampledoc>Dewdrop Cure</exampledoc>
/// <examplecapture>they draw a card before its other effects</examplecapture>
[Dependent]
public class BeforeOtherEffects : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "before its other effects"];

    public DynamicGlyph Effect { get; set; }
}
