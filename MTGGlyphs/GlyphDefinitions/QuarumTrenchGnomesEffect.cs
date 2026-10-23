namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quarum Trench Gnomes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quarum Trench Gnomes</exampledoc>
[Dependent]
public class QuarumTrenchGnomesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target plains is tapped for mana, it produces colorless mana instead of white mana"];
}
