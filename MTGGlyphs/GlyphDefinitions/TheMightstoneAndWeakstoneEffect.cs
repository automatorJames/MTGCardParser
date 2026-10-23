namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Mightstone and Weakstone // Urza, Planeswalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mightstone and Weakstone // Urza, Planeswalker</exampledoc>
[Dependent]
public class TheMightstoneAndWeakstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one —"];
}
