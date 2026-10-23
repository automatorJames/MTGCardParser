namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Magistrate's Veto, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magistrate's Veto</exampledoc>
[TokenizationOrder(-1)]
public class MagistratesVetoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["white creatures and blue creatures can't block"];
}
