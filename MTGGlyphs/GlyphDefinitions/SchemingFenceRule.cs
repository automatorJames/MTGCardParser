namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scheming Fence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scheming Fence</exampledoc>
[TokenizationOrder(-1)]
public class SchemingFenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has all activated abilities of the chosen permanent except for loyalty abilities"];
}
