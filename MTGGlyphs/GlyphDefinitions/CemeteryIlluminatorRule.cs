namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cemetery Illuminator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cemetery Illuminator</exampledoc>
/// <examplecapture>once each turn, you may cast a spell from the top of your library if it shares a card type with a card exiled with this creature</examplecapture>
[TokenizationOrder(-1)]
public class CemeteryIlluminatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may cast a spell from the top of your library if it shares a card type with a card exiled with this creature"];
}
