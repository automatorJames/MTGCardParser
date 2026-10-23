namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whelming Wave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whelming Wave</exampledoc>
[TokenizationOrder(-1)]
public class WhelmingWaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all creatures to their owners' hands except for krakens, leviathans, octopuses, and serpents"];
}
