namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Undiscovered Paradise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Undiscovered Paradise</exampledoc>
[TokenizationOrder(-1)]
public class UndiscoveredParadiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your next untap step, as you untap your permanents, return this land to its owner's hand"];
}
