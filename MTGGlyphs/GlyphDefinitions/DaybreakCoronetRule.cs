namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Daybreak Coronet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Daybreak Coronet</exampledoc>
[TokenizationOrder(-1)]
public class DaybreakCoronetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchant creature with another aura attached to it"];
}
