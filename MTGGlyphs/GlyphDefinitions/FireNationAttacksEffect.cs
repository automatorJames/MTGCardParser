namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fire Nation Attacks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fire Nation Attacks</exampledoc>
[Dependent]
public class FireNationAttacksEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature with firebending 1 attacks, add {r}"];
}
