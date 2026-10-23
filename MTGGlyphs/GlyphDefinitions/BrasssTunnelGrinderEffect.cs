namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Brass's Tunnel-Grinder // Tecutlan, the Searing Rift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brass's Tunnel-Grinder // Tecutlan, the Searing Rift</exampledoc>
[Dependent]
public class BrasssTunnelGrinderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard any number of cards, then draw that many cards plus one"];
}
