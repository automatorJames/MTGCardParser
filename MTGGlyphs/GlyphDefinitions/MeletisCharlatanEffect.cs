namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Meletis Charlatan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meletis Charlatan</exampledoc>
[Dependent]
public class MeletisCharlatanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the controller of target instant or sorcery spell copies it"];
}
