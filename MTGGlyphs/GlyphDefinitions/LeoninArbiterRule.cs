namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Leonin Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leonin Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class LeoninArbiterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may pay {2} for that player to ignore this effect until end of turn"];
}
