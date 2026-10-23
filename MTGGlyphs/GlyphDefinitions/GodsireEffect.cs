namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Godsire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Godsire</exampledoc>
[Dependent]
public class GodsireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an 8/8 beast creature token that's red, green, and white"];
}
