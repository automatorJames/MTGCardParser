namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Artifact Possession, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Artifact Possession</exampledoc>
[TokenizationOrder(-1)]
public class ArtifactPossessionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever enchanted artifact becomes tapped or a player activates an ability of enchanted artifact without {t} in its activation cost, this aura deals 2 damage to that artifact's controller"];
}
