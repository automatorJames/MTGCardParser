namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Frantic Scapegoat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frantic Scapegoat</exampledoc>
[Dependent]
public class FranticScapegoatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is suspected, you may suspect one of the other creatures"];
}
