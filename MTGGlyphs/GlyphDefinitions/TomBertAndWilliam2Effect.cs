namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[Dependent]
public class TomBertAndWilliam2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're no longer a creature"];
}
