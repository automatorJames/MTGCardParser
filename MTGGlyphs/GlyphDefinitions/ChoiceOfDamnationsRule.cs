namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Choice of Damnations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Choice of Damnations</exampledoc>
[TokenizationOrder(-1)]
public class ChoiceOfDamnationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't, that player sacrifices all but that many permanents of their choice"];
}
