namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Precipitous Drop, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Precipitous Drop</exampledoc>
[TokenizationOrder(-1)]
public class PrecipitousDropRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets -5/-5 instead as long as you've completed a dungeon"];
}
