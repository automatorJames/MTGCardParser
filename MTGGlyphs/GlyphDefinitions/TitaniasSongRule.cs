namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Titania's Song, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titania's Song</exampledoc>
[TokenizationOrder(-1)]
public class TitaniasSongRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this enchantment leaves the battlefield, this effect continues until end of turn"];
}
