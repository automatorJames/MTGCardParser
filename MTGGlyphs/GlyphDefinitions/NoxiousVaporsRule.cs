namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Noxious Vapors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noxious Vapors</exampledoc>
[TokenizationOrder(-1)]
public class NoxiousVaporsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player reveals their hand, chooses one card of each color from it, then discards all other nonland cards"];
}
