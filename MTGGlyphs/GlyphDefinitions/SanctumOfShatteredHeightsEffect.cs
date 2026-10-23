namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sanctum of Shattered Heights, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanctum of Shattered Heights</exampledoc>
/// <examplecapture>{this} deals x damage to target creature or planeswalker, where x is the number of shrines you control</examplecapture>
[Dependent]
public class SanctumOfShatteredHeightsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage to target creature or planeswalker, where x is the number of shrines you control"];
}
