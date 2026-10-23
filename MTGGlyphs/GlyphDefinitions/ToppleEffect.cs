namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Topple, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Topple</exampledoc>
[Dependent]
public class ToppleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more creatures are tied for greatest power, target any one of them"];
}
