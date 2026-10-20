namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A sentence that continues the previous one's effect, in order: "then exile this card", "then exile it", "then shuffle", "then put a charge counter on this artifact". The effect is any glyph that resolves.</summary>
/// <exampledoc>Altar of Shadows</exampledoc>
/// <examplecapture>then put a charge counter on this artifact</examplecapture>
public class ThenEffect : Glyph
{
    public override Nib[] Nibs => ["then", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
