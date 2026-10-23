namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pompous Gadabout, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pompous Gadabout</exampledoc>
[TokenizationOrder(-1)]
public class PompousGadaboutRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be blocked by creatures that don't have a name"];
}
