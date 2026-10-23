namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quest for Pure Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quest for Pure Flame</exampledoc>
/// <examplecapture>if any source you control would deal damage to a permanent or player this turn, it deals double that damage to that permanent or player instead</examplecapture>
[Dependent]
public class QuestForPureFlameEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any source you control would deal damage to a permanent or player this turn, it deals double that damage to that permanent or player instead"];
}
