namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kyren Toy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kyren Toy</exampledoc>
[Dependent]
public class KyrenToyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an amount of {c} equal to x plus one"];
}
