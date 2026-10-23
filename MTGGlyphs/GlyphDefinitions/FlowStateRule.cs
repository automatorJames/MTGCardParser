namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flow State, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flow State</exampledoc>
/// <examplecapture>if there is an instant card and a sorcery card in your graveyard, instead put two of them into your hand and the rest on the bottom of your library in any order</examplecapture>
[TokenizationOrder(-1)]
public class FlowStateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there is an instant card and a sorcery card in your graveyard, instead put two of them into your hand and the rest on the bottom of your library in any order"];
}
