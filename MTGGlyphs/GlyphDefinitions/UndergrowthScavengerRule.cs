namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Undergrowth Scavenger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Undergrowth Scavenger</exampledoc>
[TokenizationOrder(-1)]
public class UndergrowthScavengerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with a number of +1/+1 counters on it equal to the number of creature cards in all graveyards"];
}
