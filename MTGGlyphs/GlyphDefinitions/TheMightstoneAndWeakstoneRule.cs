namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Mightstone and Weakstone // Urza, Planeswalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mightstone and Weakstone // Urza, Planeswalker</exampledoc>
[TokenizationOrder(-1)]
public class TheMightstoneAndWeakstoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the mightstone and weakstone enters, choose one —"];
}
