namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Secret Identity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Secret Identity</exampledoc>
[TokenizationOrder(-1)]
public class SecretIdentityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• reveal — until end of turn, target creature you control becomes a hero with base power and toughness 3/4 and gains flying and vigilance"];
}
