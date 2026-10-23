namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vengevine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vengevine</exampledoc>
/// <examplecapture>if it's the second creature spell you cast this turn, you may return this card from your graveyard to the battlefield</examplecapture>
[Dependent]
public class VengevineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the second creature spell you cast this turn, you may return this card from your graveyard to the battlefield"];
}
