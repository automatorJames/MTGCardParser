namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hymn of the Faller, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hymn of the Faller</exampledoc>
[TokenizationOrder(-1)]
public class HymnOfTheFallerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["void — if a nonland permanent left the battlefield this turn or a spell was warped this turn, draw another card"];
}
