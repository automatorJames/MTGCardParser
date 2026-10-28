namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Magical Hack, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magical Hack</exampledoc>
[TokenizationOrder(-1)]
public class MagicalHackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(for example, you may change \"swampwalk\" to \"plainswalk"];
}
