namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Honest Work, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Honest Work</exampledoc>
[TokenizationOrder(-1)]
public class HonestWorkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature loses all abilities and is a citizen with base power and toughness 1/1 and \"{t}: add {c}\" named humble merchant"];
}
