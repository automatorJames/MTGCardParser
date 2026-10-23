namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emblazoned Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emblazoned Golem</exampledoc>
[TokenizationOrder(-1)]
public class EmblazonedGolem2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only colored mana on x"];
}
