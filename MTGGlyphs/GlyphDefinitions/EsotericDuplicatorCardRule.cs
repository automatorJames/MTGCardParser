namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Esoteric Duplicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Esoteric Duplicator</exampledoc>
[TokenizationOrder(-1)]
public class EsotericDuplicatorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you sacrifice this artifact or another artifact, you may pay {2}"];
}
