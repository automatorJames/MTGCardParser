namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rebirth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rebirth</exampledoc>
[TokenizationOrder(-1)]
public class RebirthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may ante the top card of their library"];
}
