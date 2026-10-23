namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Szadek, Lord of Secrets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Szadek, Lord of Secrets</exampledoc>
[TokenizationOrder(-1)]
public class SzadekLordOfSecretsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if szadek would deal combat damage to a player, instead put that many +1/+1 counters on szadek and that player mills that many cards"];
}
