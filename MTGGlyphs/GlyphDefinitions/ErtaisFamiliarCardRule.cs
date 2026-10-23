namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ertai's Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Familiar</exampledoc>
[TokenizationOrder(-1)]
public class ErtaisFamiliarCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature phases out or leaves the battlefield, mill three cards"];
}
