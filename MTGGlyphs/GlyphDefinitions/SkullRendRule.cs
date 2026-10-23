namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skull Rend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skull Rend</exampledoc>
[TokenizationOrder(-1)]
public class SkullRendRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players each discard two cards at random"];
}
