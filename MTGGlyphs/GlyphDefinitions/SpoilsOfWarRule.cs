namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spoils of War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spoils of War</exampledoc>
[TokenizationOrder(-1)]
public class SpoilsOfWarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x is the number of artifact and/or creature cards in an opponent's graveyard as you cast this spell"];
}
