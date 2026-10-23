namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avatar of Woe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avatar of Woe</exampledoc>
[TokenizationOrder(-1)]
public class AvatarOfWoeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are ten or more creature cards total in all graveyards, this spell costs {6} less to cast"];
}
