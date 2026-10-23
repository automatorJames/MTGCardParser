namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winding Constrictor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winding Constrictor</exampledoc>
[TokenizationOrder(-1)]
public class WindingConstrictorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would get one or more counters, you get that many plus one of each of those kinds of counters instead"];
}
