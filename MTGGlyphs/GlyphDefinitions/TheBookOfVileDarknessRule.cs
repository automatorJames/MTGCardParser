namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Book of Vile Darkness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Book of Vile Darkness</exampledoc>
[TokenizationOrder(-1)]
public class TheBookOfVileDarknessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, exile", Nib.This, "and artifacts you control named eye of vecna and hand of vecna: create vecna, a legendary 8/8 black zombie god creature token with indestructible and all triggered abilities of the exiled cards"];
}
