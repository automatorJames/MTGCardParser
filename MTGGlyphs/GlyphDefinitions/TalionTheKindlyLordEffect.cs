namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Talion, the Kindly Lord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Talion, the Kindly Lord</exampledoc>
/// <examplecapture>power, or toughness equal to the chosen number, that player loses 2 life and you draw a card</examplecapture>
[Dependent]
public class TalionTheKindlyLordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["power, or toughness equal to the chosen number, that player loses 2 life and you draw a card"];
}
