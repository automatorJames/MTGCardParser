namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tenth District Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tenth District Hero</exampledoc>
/// <examplecapture>if this creature is a detective, it becomes a legendary creature named mileva, the stalwart, it has base power and toughness 5/5, and it gains "other creatures you control have indestructible</examplecapture>
[Dependent]
public class TenthDistrictHeroEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is a detective, it becomes a legendary creature named mileva, the stalwart, it has base power and toughness 5/5, and it gains \"other creatures you control have indestructible"];
}
