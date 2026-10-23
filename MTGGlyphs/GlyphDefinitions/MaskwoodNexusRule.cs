namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Maskwood Nexus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maskwood Nexus</exampledoc>
[TokenizationOrder(-1)]
public class MaskwoodNexusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control are every creature type"];
}
