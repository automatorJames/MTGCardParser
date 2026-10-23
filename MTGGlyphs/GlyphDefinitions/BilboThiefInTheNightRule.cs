namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bilbo, Thief in the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bilbo, Thief in the Night</exampledoc>
[TokenizationOrder(-1)]
public class BilboThiefInTheNightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an instant or sorcery spell cast this way would be put into your graveyard, exile it instead"];
}
