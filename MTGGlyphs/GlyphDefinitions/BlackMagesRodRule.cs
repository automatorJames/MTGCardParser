namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Black Mage's Rod, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Black Mage's Rod</exampledoc>
/// <examplecapture>equipped creature gets +1/+0, has "whenever you cast a noncreature spell, this creature deals 1 damage to each opponent," and is a wizard in addition to its other types</examplecapture>
[TokenizationOrder(-1)]
public class BlackMagesRodRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +1/+0, has \"whenever you cast a noncreature spell, this creature deals 1 damage to each opponent,\" and is a wizard in addition to its other types"];
}
