namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wail of the Forgotten, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wail of the Forgotten</exampledoc>
[TokenizationOrder(-1)]
public class WailOfTheForgottenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are eight or more permanent cards in your graveyard as you cast this spell, choose one or more instead"];
}
