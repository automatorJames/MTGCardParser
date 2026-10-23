namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Captain Marvel, Earth's Protector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Captain Marvel, Earth's Protector</exampledoc>
[Dependent]
public class CaptainMarvelEarthsProtectorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and an indestructible counter on captain marvel"];
}
