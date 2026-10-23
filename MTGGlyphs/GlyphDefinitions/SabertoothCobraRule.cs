namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sabertooth Cobra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sabertooth Cobra</exampledoc>
[TokenizationOrder(-1)]
public class SabertoothCobraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the player gets another poison counter at the beginning of their next upkeep unless they pay {2} before that step"];
}
