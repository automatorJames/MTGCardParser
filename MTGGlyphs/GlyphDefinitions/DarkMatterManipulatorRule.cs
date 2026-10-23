namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dark Matter Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dark Matter Manipulator</exampledoc>
[TokenizationOrder(-1)]
public class DarkMatterManipulatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+0 for every seven cards in your graveyard"];
}
