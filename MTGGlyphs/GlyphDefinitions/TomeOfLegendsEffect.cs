namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tome of Legends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tome of Legends</exampledoc>
[Dependent]
public class TomeOfLegendsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a page counter on this artifact"];
}
