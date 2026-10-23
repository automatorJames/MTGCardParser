namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Giant Beaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Beaver</exampledoc>
[Dependent]
public class GiantBeaverEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature that saddled it this turn"];
}
