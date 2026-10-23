namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Power Sink, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Power Sink</exampledoc>
[TokenizationOrder(-1)]
public class PowerSinkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player doesn't, they tap all lands with mana abilities they control and lose all unspent mana"];
}
