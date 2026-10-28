namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra's Pyreling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Pyreling</exampledoc>
[TokenizationOrder(-1)]
public class ChandrasPyrelingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control deals noncombat damage to an opponent, this creature gets +1/+0 and gains double strike until end of turn"];
}
