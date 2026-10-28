namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Platinum Emperion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Platinum Emperion</exampledoc>
[Dependent]
public class PlatinumEmperionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't gain or lose life"];
}
