namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niko Defies Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko Defies Destiny</exampledoc>
[TokenizationOrder(-1)]
public class NikoDefiesDestinyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i — you gain 2 life for each foretold card you own in exile"];
}
