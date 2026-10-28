namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scrambleverse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrambleverse</exampledoc>
[TokenizationOrder(-1)]
public class ScrambleverseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each nonland permanent, choose a player at random"];
}
