namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bronze Horse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronze Horse</exampledoc>
[TokenizationOrder(-1)]
public class BronzeHorseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control another creature, prevent all damage that would be dealt to this creature by spells that target it"];
}
