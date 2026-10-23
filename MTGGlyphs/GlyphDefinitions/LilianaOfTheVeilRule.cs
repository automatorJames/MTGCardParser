namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liliana of the Veil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana of the Veil</exampledoc>
[TokenizationOrder(-1)]
public class LilianaOfTheVeilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player sacrifices all permanents in the pile of their choice"];
}
