namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Timmerian Fiends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timmerian Fiends</exampledoc>
[TokenizationOrder(-1)]
public class TimmerianFiendsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player doesn't, exchange ownership of that artifact and", Nib.This];
}
