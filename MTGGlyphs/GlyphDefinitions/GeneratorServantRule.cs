namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Generator Servant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Generator Servant</exampledoc>
[TokenizationOrder(-1)]
public class GeneratorServantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of that mana is spent on a creature spell, it gains haste until end of turn"];
}
