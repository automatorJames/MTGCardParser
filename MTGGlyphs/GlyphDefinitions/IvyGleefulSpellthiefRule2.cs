namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ivy, Gleeful Spellthief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ivy, Gleeful Spellthief</exampledoc>
[TokenizationOrder(-1)]
public class IvyGleefulSpellthiefRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy targets ivy"];
}
