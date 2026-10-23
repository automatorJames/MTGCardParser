namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Parry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Parry</exampledoc>
[TokenizationOrder(-1)]
public class SoulParryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage one or two target creatures would deal this turn"];
}
