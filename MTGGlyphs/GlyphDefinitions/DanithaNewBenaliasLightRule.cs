namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Danitha, New Benalia's Light, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Danitha, New Benalia's Light</exampledoc>
[TokenizationOrder(-1)]
public class DanithaNewBenaliasLightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast an aura or equipment spell from your graveyard"];
}
