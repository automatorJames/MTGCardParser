namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of White Tiger, Ava Ayala, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>White Tiger, Ava Ayala</exampledoc>
[TokenizationOrder(-1)]
public class WhiteTigerAvaAyalaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["power-up — {5}{g}: put a +1/+1 counter on white tiger and create the tiger god, a legendary 4/4 green cat god creature token with \"the tiger god can't be blocked by more than one creature"];
}
