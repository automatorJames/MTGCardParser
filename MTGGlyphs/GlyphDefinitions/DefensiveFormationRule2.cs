namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Defensive Formation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defensive Formation</exampledoc>
[TokenizationOrder(-1)]
public class DefensiveFormationRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can divide that creature's combat damage as you choose among any of the creatures blocking it"];
}
