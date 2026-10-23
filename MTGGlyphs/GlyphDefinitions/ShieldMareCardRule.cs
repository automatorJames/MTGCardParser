namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shield Mare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shield Mare</exampledoc>
[TokenizationOrder(-1)]
public class ShieldMareCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature enters or becomes the target of a spell or ability an opponent controls, you gain 3 life"];
}
