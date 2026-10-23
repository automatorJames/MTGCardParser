namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Coffin Queen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coffin Queen</exampledoc>
[Dependent]
public class CoffinQueenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that creature"];
}
