namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gruesome Menagerie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gruesome Menagerie</exampledoc>
[TokenizationOrder(-1)]
public class GruesomeMenagerieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature card with mana value 1 in your graveyard, then do the same for creature cards with mana value 2 and 3"];
}
