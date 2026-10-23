namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chaos Lord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Lord</exampledoc>
[Dependent]
public class ChaosLordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent gains control of this creature if the number of permanents is even"];
}
