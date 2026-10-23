namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pit Automaton, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pit Automaton</exampledoc>
[Dependent]
public class PitAutomatonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you next activate an exhaust ability that isn't a mana ability this turn, copy it"];
}
