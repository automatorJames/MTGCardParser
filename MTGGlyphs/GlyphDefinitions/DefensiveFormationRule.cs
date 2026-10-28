namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Defensive Formation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defensive Formation</exampledoc>
[TokenizationOrder(-1)]
public class DefensiveFormationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["rather than the attacking player, you assign the combat damage of each creature attacking you"];
}
