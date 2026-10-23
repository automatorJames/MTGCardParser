namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Primal Clay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Primal Clay</exampledoc>
/// <examplecapture>as this creature enters, it becomes your choice of a 3/3 artifact creature, a 2/2 artifact creature with flying, or a 1/6 wall artifact creature with defender in addition to its other types</examplecapture>
[TokenizationOrder(-1)]
public class PrimalClayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this creature enters, it becomes your choice of a 3/3 artifact creature, a 2/2 artifact creature with flying, or a 1/6 wall artifact creature with defender in addition to its other types"];
}
