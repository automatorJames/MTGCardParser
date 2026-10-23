namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ironclaw Curse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ironclaw Curse</exampledoc>
[TokenizationOrder(-1)]
public class IronclawCurseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't block creatures with power equal to or greater than the enchanted creature's toughness"];
}
