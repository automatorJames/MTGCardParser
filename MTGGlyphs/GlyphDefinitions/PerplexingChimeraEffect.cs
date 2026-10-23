namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Perplexing Chimera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Perplexing Chimera</exampledoc>
[Dependent]
public class PerplexingChimeraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exchange control of this creature and that spell"];
}
