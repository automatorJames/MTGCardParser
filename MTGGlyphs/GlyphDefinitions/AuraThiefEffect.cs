namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aura Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Thief</exampledoc>
[Dependent]
public class AuraThiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you don't get to move auras"];
}
