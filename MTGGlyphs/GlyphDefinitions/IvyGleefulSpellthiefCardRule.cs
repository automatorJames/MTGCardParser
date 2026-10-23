namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ivy, Gleeful Spellthief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ivy, Gleeful Spellthief</exampledoc>
[TokenizationOrder(-1)]
public class IvyGleefulSpellthiefCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player casts a spell that targets only a single creature other than ivy, you may copy that spell"];
}
