namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dragon's Approach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragon's Approach</exampledoc>
[TokenizationOrder(-1)]
public class DragonsApproachRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile this spell and four cards named", Nib.This, "from your graveyard"];
}
