namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ecological Appreciation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ecological Appreciation</exampledoc>
/// <examplecapture>search your library and graveyard for up to four creature cards with different names that each have mana value x or less and reveal them</examplecapture>
[TokenizationOrder(-1)]
public class EcologicalAppreciationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library and graveyard for up to four creature cards with different names that each have mana value x or less and reveal them"];
}
