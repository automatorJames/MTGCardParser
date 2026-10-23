namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hallowed Respite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hallowed Respite</exampledoc>
[TokenizationOrder(-1)]
public class HallowedRespiteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it entered under your control, put a +1/+1 counter on it"];
}
