namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thought Prison, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Prison</exampledoc>
[Dependent]
public class ThoughtPrisonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact deals 2 damage to that player"];
}
