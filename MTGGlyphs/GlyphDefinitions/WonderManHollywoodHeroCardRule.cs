namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[TokenizationOrder(-1)]
public class WonderManHollywoodHeroCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(activate each power-up ability only"];
}
