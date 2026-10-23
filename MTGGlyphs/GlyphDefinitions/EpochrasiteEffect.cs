namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Epochrasite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Epochrasite</exampledoc>
[Dependent]
public class EpochrasiteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they may cast this card without paying its mana cost"];
}
