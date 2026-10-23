namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pestilent Souleater, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pestilent Souleater</exampledoc>
[TokenizationOrder(-1)]
public class PestilentSouleaterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a creature with infect deals damage to creatures in the form of -1/-1 counters and to players in the form of poison counters"];
}
