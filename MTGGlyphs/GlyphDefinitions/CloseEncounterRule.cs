namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Close Encounter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Close Encounter</exampledoc>
[TokenizationOrder(-1)]
public class CloseEncounterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, choose a creature you control or a warped creature card you own in exile"];
}
