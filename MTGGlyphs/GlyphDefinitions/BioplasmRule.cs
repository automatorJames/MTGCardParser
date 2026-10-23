namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bioplasm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bioplasm</exampledoc>
/// <examplecapture>if it's a creature card, this creature gets +x/+y until end of turn, where x is the exiled creature card's power and y is its toughness</examplecapture>
[TokenizationOrder(-1)]
public class BioplasmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a creature card, this creature gets +x/+y until end of turn, where x is the exiled creature card's power and y is its toughness"];
}
