namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mistcaller, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistcaller</exampledoc>
[Dependent]
public class MistcallerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if a nontoken creature would enter and it wasn't cast, exile it instead"];
}
