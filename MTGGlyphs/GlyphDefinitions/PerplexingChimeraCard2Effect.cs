namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Perplexing Chimera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Perplexing Chimera</exampledoc>
[Dependent]
public class PerplexingChimeraCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the spell becomes a permanent, you control that permanent"];
}
