namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unholy Citadel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unholy Citadel</exampledoc>
[TokenizationOrder(-1)]
public class UnholyCitadelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["black legendary creatures you control have \"bands with other legendary creatures"];
}
