namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Defiant Vanguard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Defiant Vanguard</exampledoc>
[Dependent]
public class DefiantVanguardEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at end of combat, destroy it and all creatures it blocked this turn"];
}
