namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Unstable Glyphbridge // Sandswirl Wanderglyph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unstable Glyphbridge // Sandswirl Wanderglyph</exampledoc>
[Dependent]
public class UnstableGlyphbridgeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it, for each player, choose a creature with power 2 or less that player controls"];
}
