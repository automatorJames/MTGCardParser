namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Medomai's Prophecy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Medomai's Prophecy</exampledoc>
[TokenizationOrder(-1)]
public class MedomaisProphecyCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iii — when you cast a spell with the chosen name for the first time this turn, draw two cards"];
}
