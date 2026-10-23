namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Phyrexian Unlife, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Unlife</exampledoc>
[Dependent]
public class PhyrexianUnlifeCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage is dealt to you in the form of poison counters"];
}
