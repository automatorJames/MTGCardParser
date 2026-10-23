namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gisa and Geralf, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gisa and Geralf</exampledoc>
[TokenizationOrder(-1)]
public class GisaAndGeralfRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast a zombie creature spell from your graveyard"];
}
