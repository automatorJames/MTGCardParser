namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Athreos, Shroud-Veiled, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Athreos, Shroud-Veiled</exampledoc>
[Dependent]
public class AthreosShroudVeiledEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to the battlefield under your control"];
}
