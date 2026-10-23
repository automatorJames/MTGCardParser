namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Autarch Mammoth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Autarch Mammoth</exampledoc>
[TokenizationOrder(-1)]
public class AutarchMammothCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature enters and whenever it attacks while saddled, create a 3/3 green elephant creature token"];
}
