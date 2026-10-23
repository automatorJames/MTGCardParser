namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bronze Bombshell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronze Bombshell</exampledoc>
[Dependent]
public class BronzeBombshellEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player sacrifices it"];
}
