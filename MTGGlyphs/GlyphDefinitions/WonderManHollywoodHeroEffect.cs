namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[Dependent]
public class WonderManHollywoodHeroEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate each power-up ability only"];
}
