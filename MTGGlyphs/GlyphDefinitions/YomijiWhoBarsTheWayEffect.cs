namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Yomiji, Who Bars the Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yomiji, Who Bars the Way</exampledoc>
[Dependent]
public class YomijiWhoBarsTheWayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to its owner's hand"];
}
