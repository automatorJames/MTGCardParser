namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Repeated Reverberation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Repeated Reverberation</exampledoc>
[TokenizationOrder(-1)]
public class RepeatedReverberationCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you next cast an instant spell, cast a sorcery spell, or activate a loyalty ability this turn, copy that spell or ability twice"];
}
