namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Enduring Courage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enduring Courage</exampledoc>
[Dependent]
public class EnduringCourageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's not a creature"];
}
