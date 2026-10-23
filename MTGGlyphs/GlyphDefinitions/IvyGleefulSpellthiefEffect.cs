namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ivy, Gleeful Spellthief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ivy, Gleeful Spellthief</exampledoc>
[Dependent]
public class IvyGleefulSpellthiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy that spell"];
}
