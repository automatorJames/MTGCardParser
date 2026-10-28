namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mind Bend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mind Bend</exampledoc>
[TokenizationOrder(-1)]
public class MindBendRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(for example, you may change \"nonblack creature\" to \"nongreen creature\" or \"forestwalk\" to \"islandwalk"];
}
