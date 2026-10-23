namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of the Wild Hunt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of the Wild Hunt</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfTheWildHunt2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each wolf tapped this way deals damage equal to its power to target creature"];
}
