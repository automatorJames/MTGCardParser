namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invasion of Tarkir // Defiant Thundermaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Tarkir // Defiant Thundermaw</exampledoc>
[Dependent]
public class InvasionOfTarkirEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can be 0"];
}
