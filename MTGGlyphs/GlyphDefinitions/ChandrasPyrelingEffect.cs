namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra's Pyreling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Pyreling</exampledoc>
[Dependent]
public class ChandrasPyrelingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+0 and gains double strike until end of turn"];
}
