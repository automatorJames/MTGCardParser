namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vengeful Pharaoh, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vengeful Pharaoh</exampledoc>
/// <examplecapture>if this card is in your graveyard, destroy target attacking creature, then put this card on top of your library</examplecapture>
[Dependent]
public class VengefulPharaohEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your graveyard, destroy target attacking creature, then put this card on top of your library"];
}
