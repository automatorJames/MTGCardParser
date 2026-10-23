namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Temple of the Dragon Queen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temple of the Dragon Queen</exampledoc>
[TokenizationOrder(-1)]
public class TempleOfTheDragonQueenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this land enters tapped unless you revealed a dragon card this way or you control a dragon"];
}
