namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gonti's Machinations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gonti's Machinations</exampledoc>
[TokenizationOrder(-1)]
public class GontisMachinationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage causes loss of life"];
}
