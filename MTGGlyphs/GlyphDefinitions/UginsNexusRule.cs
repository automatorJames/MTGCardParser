namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ugin's Nexus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ugin's Nexus</exampledoc>
[TokenizationOrder(-1)]
public class UginsNexusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a player would begin an extra turn, that player skips that turn instead"];
}
