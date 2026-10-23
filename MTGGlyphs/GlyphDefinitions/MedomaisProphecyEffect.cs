namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Medomai's Prophecy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Medomai's Prophecy</exampledoc>
[Dependent]
public class MedomaisProphecyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw two cards"];
}
