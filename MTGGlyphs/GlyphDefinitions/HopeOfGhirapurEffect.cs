namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hope of Ghirapur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hope of Ghirapur</exampledoc>
[Dependent]
public class HopeOfGhirapurEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, target player who was dealt combat damage by", Nib.This, "this turn can't cast noncreature spells"];
}
