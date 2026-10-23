namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curator of Sun's Creation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curator of Sun's Creation</exampledoc>
[Dependent]
public class CuratorOfSunsCreationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discover again for the same value"];
}
