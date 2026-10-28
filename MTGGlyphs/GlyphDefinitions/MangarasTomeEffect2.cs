namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mangara's Tome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara's Tome</exampledoc>
[Dependent]
public class MangarasTomeEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time you would draw a card this turn, instead put the top card of the exiled pile into its owner's hand"];
}
