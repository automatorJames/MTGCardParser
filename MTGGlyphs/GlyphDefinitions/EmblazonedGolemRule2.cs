namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emblazoned Golem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emblazoned Golem</exampledoc>
[TokenizationOrder(-1)]
public class EmblazonedGolemRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["no more than one mana of each color may be spent this way"];
}
