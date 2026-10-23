namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vaevictis Asmadi, the Dire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vaevictis Asmadi, the Dire</exampledoc>
/// <examplecapture>each player who sacrificed a permanent this way reveals the top card of their library, then puts it onto the battlefield if it's a permanent card</examplecapture>
[TokenizationOrder(-1)]
public class VaevictisAsmadiTheDireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who sacrificed a permanent this way reveals the top card of their library, then puts it onto the battlefield if it's a permanent card"];
}
