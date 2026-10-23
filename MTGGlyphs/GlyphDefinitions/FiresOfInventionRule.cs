namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fires of Invention, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fires of Invention</exampledoc>
[TokenizationOrder(-1)]
public class FiresOfInventionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can cast spells only during your turn and you can cast no more than two spells each turn"];
}
