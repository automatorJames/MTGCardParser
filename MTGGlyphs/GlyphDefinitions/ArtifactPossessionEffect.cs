namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Artifact Possession, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Artifact Possession</exampledoc>
/// <examplecapture>this aura deals 2 damage to that artifact's controller</examplecapture>
[Dependent]
public class ArtifactPossessionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this aura deals 2 damage to that artifact's controller"];
}
