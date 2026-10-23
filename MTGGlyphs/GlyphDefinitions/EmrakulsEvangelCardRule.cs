namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emrakul's Evangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul's Evangel</exampledoc>
[TokenizationOrder(-1)]
public class EmrakulsEvangelCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, sacrifice this creature and any number of other non-eldrazi creatures: create a 3/2 colorless eldrazi horror creature token for each creature sacrificed this way"];
}
