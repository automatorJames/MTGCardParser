namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Astrologian's Planisphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Astrologian's Planisphere</exampledoc>
/// <examplecapture>equipped creature is a wizard in addition to its other types and has "whenever you cast a noncreature spell and whenever you draw your third card each turn, put a +1/+1 counter on this creature</examplecapture>
[TokenizationOrder(-1)]
public class AstrologiansPlanisphereRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature is a wizard in addition to its other types and has \"whenever you cast a noncreature spell and whenever you draw your third card each turn, put a +1/+1 counter on this creature"];
}
