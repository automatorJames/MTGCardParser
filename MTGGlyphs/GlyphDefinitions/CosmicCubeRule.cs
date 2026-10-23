namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cosmic Cube, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cosmic Cube</exampledoc>
/// <examplecapture>you may cast a spell from among them with mana value less than or equal to the greatest power among attacking creatures you control without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class CosmicCubeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell from among them with mana value less than or equal to the greatest power among attacking creatures you control without paying its mana cost"];
}
