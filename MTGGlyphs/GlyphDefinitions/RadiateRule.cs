namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Radiate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Radiate</exampledoc>
[TokenizationOrder(-1)]
public class RadiateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target instant or sorcery spell that targets only a single permanent or player"];
}
