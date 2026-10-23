namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Clash of the Eikons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clash of the Eikons</exampledoc>
[Dependent]
public class ClashOfTheEikonsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["removing lore counters doesn't cause chapter abilities to trigger"];
}
