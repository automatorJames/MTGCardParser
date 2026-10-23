namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of As Foretold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>As Foretold</exampledoc>
/// <examplecapture>once each turn, you may pay {0} rather than pay the mana cost for a spell you cast with mana value x or less, where x is the number of time counters on this enchantment</examplecapture>
[TokenizationOrder(-1)]
public class AsForetoldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may pay {0} rather than pay the mana cost for a spell you cast with mana value x or less, where x is the number of time counters on this enchantment"];
}
