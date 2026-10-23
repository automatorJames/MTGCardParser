namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Savor the Moment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Savor the Moment</exampledoc>
[TokenizationOrder(-1)]
public class SavorTheMomentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["skip the untap step of that turn"];
}
