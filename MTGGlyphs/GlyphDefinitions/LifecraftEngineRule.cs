namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lifecraft Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lifecraft Engine</exampledoc>
[TokenizationOrder(-1)]
public class LifecraftEngineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control of the chosen type other than this vehicle gets +1/+1"];
}
