namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of the Wild Hunt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of the Wild Hunt</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfTheWildHuntRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature deals damage equal to its power divided as its controller chooses among any number of those wolves"];
}
