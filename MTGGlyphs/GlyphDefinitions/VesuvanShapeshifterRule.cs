namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vesuvan Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vesuvan Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class VesuvanShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this creature enters or is turned face up, you may choose another creature on the battlefield"];
}
