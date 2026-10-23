namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of False Dawn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>False Dawn</exampledoc>
[TokenizationOrder(-1)]
public class FalseDawnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, spells and abilities you control that would add colored mana instead add that much white mana"];
}
