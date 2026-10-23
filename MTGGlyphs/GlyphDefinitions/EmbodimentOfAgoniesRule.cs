namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Embodiment of Agonies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Embodiment of Agonies</exampledoc>
[TokenizationOrder(-1)]
public class EmbodimentOfAgoniesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with a +1/+1 counter on it for each different mana cost among nonland cards in your graveyard"];
}
