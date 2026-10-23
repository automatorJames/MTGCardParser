namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raise the Draugr, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raise the Draugr</exampledoc>
/// <examplecapture>• return two target creature cards that share a creature type from your graveyard to your hand</examplecapture>
[TokenizationOrder(-1)]
public class RaiseTheDraugrRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• return two target creature cards that share a creature type from your graveyard to your hand"];
}
