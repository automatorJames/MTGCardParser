namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Power Artifact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Power Artifact</exampledoc>
[TokenizationOrder(-1)]
public class PowerArtifactRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted artifact's activated abilities cost {2} less to activate"];
}
