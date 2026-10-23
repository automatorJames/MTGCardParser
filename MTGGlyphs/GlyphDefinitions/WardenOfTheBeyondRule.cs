namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warden of the Beyond, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warden of the Beyond</exampledoc>
[TokenizationOrder(-1)]
public class WardenOfTheBeyondRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+2 as long as an opponent owns a card in exile"];
}
