namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thorin, Mountain-king, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thorin, Mountain-king</exampledoc>
[TokenizationOrder(-1)]
public class ThorinMountainKingCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when one or more equipment become attached to that creature this way, that creature deals damage equal to its power to up to one target creature"];
}
