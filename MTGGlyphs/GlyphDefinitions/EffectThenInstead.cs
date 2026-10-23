namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"… instead" closing a replacement effect: "you gain 5 life instead", "each opponent discards a card instead".</summary>
/// <exampledoc>Words of Worship</exampledoc>
/// <examplecapture>you gain 5 life instead</examplecapture>
[Dependent]
public class EffectThenInstead : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "instead"];

    public DynamicGlyph Effect { get; set; }
}
