namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nebuchadnezzar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nebuchadnezzar</exampledoc>
[TokenizationOrder(-1)]
public class NebuchadnezzarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player discards all cards with that name revealed this way"];
}
