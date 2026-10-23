namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master Biomancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master Biomancer</exampledoc>
/// <examplecapture>each other creature you control enters with a number of additional +1/+1 counters on it equal to this creature's power and as a mutant in addition to its other types</examplecapture>
[TokenizationOrder(-1)]
public class MasterBiomancerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other creature you control enters with a number of additional +1/+1 counters on it equal to this creature's power and as a mutant in addition to its other types"];
}
