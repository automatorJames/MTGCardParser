namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Transmute Artifact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transmute Artifact</exampledoc>
[TokenizationOrder(-1)]
public class TransmuteArtifactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's greater, you may pay {x}, where x is the difference"];
}
