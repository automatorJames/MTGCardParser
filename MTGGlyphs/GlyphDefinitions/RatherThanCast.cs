namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Suspend's reminder: "rather than cast this card from your hand, you may pay {r} and exile it with one time counter on it" - an alternative to casting the card, with the effect that replaces it.</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>rather than cast this card from your hand, you may pay {r} and exile it with a time counter on it</examplecapture>
public class RatherThanCast : Glyph
{
    public override Nib[] Nibs => ["rather than cast this card from your hand,", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
