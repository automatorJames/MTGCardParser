namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Caravan Vigil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Caravan Vigil</exampledoc>
[TokenizationOrder(-1)]
public class CaravanVigilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["morbid — you may put that card onto the battlefield instead of putting it into your hand if a creature died this turn"];
}
