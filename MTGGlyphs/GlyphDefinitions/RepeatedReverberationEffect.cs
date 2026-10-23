namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Repeated Reverberation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Repeated Reverberation</exampledoc>
[Dependent]
public class RepeatedReverberationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast a sorcery spell, or activate a loyalty ability this turn, copy that spell or ability twice"];
}
