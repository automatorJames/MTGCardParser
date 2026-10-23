namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thought Prison, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Prison</exampledoc>
[TokenizationOrder(-1)]
public class ThoughtPrisonCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player casts a spell that shares a color or mana value with the exiled card, this artifact deals 2 damage to that player"];
}
