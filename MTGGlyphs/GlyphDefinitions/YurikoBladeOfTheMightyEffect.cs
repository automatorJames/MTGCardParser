namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Yuriko, Blade of the Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yuriko, Blade of the Mighty</exampledoc>
[Dependent]
public class YurikoBladeOfTheMightyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gains double strike until end of turn"];
}
