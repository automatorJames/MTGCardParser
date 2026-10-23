namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Herald of Anafenza, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Anafenza</exampledoc>
[Dependent]
public class HeraldOfAnafenzaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white warrior creature token"];
}
