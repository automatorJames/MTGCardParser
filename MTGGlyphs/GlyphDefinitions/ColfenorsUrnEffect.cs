namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Colfenor's Urn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Colfenor's Urn</exampledoc>
[Dependent]
public class ColfenorsUrnEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if three or more cards have been exiled with this artifact, sacrifice it"];
}
