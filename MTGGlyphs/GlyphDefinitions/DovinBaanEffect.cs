namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dovin Baan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dovin Baan</exampledoc>
[Dependent]
public class DovinBaanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"your opponents can't untap more than two permanents during their untap steps"];
}
