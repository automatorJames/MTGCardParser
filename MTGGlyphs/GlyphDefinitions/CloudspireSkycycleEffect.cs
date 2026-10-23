namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cloudspire Skycycle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cloudspire Skycycle</exampledoc>
[Dependent]
public class CloudspireSkycycleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute two +1/+1 counters among one or two other target vehicles and/or creatures you control"];
}
