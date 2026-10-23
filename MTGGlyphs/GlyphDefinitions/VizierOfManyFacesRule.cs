namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vizier of Many Faces, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizier of Many Faces</exampledoc>
/// <examplecapture>you may have this creature enter as a copy of any creature on the battlefield, except if this creature was embalmed, the token has no mana cost, it's white, and it's a zombie in addition to its other types</examplecapture>
[TokenizationOrder(-1)]
public class VizierOfManyFacesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature enter as a copy of any creature on the battlefield, except if this creature was embalmed, the token has no mana cost, it's white, and it's a zombie in addition to its other types"];
}
