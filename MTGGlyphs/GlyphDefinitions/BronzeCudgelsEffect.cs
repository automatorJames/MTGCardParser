namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bronze Cudgels, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronze Cudgels</exampledoc>
[Dependent]
public class BronzeCudgelsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, equipped creature gets +x/+0, where x is the number of times this ability has resolved this turn"];
}
