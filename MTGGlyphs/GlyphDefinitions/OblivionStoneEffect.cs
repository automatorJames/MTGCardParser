namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oblivion Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oblivion Stone</exampledoc>
[Dependent]
public class OblivionStoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each nonland permanent without a fate counter on it, then remove all fate counters from all permanents"];
}
