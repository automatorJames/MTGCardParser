namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Storyweave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storyweave</exampledoc>
[TokenizationOrder(-1)]
public class StoryweaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time one or more enchantment creatures you control enter this turn, each enters with two additional +1/+1 counters on it"];
}
