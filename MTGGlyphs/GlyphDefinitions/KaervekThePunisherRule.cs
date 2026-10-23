namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaervek, the Punisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaervek, the Punisher</exampledoc>
[TokenizationOrder(-1)]
public class KaervekThePunisherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copies of permanent spells become tokens"];
}
