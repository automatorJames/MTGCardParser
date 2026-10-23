namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ennis, Debate Moderator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ennis, Debate Moderator</exampledoc>
[Dependent]
public class EnnisDebateModeratorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if one or more cards were put into exile this turn, put a +1/+1 counter on ennis"];
}
