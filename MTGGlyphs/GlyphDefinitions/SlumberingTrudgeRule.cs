namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Slumbering Trudge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slumbering Trudge</exampledoc>
[TokenizationOrder(-1)]
public class SlumberingTrudgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with a number of stun counters on it equal to three minus x"];
}
