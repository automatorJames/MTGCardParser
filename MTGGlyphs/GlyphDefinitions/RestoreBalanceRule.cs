namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Restore Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Restore Balance</exampledoc>
[TokenizationOrder(-1)]
public class RestoreBalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players sacrifice creatures and discard cards the same way"];
}
