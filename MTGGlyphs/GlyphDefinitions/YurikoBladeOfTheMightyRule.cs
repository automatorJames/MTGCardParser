namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yuriko, Blade of the Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yuriko, Blade of the Mighty</exampledoc>
[TokenizationOrder(-1)]
public class YurikoBladeOfTheMightyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control attacks a player alone, it gains double strike until end of turn"];
}
