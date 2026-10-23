namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Signal the Clans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Signal the Clans</exampledoc>
[TokenizationOrder(-1)]
public class SignalTheClansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you reveal three cards with different names, choose one of them at random and put that card into your hand"];
}
