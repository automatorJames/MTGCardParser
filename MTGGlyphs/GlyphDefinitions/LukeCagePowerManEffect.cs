namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Luke Cage, Power Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luke Cage, Power Man</exampledoc>
[Dependent]
public class LukeCagePowerManEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy him"];
}
