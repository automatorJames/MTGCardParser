namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Scrap Welder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrap Welder</exampledoc>
[Dependent]
public class ScrapWelderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target artifact card with mana value less than x from your graveyard to the battlefield"];
}
