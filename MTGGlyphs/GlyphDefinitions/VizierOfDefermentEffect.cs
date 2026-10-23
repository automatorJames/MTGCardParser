namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vizier of Deferment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizier of Deferment</exampledoc>
[Dependent]
public class VizierOfDefermentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile target creature if it attacked or blocked this turn"];
}
