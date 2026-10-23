namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winds of Rath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winds of Rath</exampledoc>
[TokenizationOrder(-1)]
public class WindsOfRathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't enchanted"];
}
