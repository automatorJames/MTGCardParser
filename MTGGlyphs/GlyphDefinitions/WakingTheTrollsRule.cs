namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Waking the Trolls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Waking the Trolls</exampledoc>
[TokenizationOrder(-1)]
public class WakingTheTrollsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they control fewer lands than you, create a number of 4/4 green troll warrior creature tokens with trample equal to the difference"];
}
