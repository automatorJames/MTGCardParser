namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brothers Yamazaki, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brothers Yamazaki</exampledoc>
[TokenizationOrder(-1)]
public class BrothersYamazakiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are exactly two permanents named", Nib.This, "on the battlefield, the \"legend rule\" doesn't apply to them"];
}
