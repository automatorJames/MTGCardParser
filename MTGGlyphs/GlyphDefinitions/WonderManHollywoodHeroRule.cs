namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[TokenizationOrder(-1)]
public class WonderManHollywoodHeroRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each power-up ability of permanents you control can be activated an additional time"];
}
