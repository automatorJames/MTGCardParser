namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Master of Winds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Winds</exampledoc>
/// <examplecapture>sorcery, or wizard spell, you may have this creature's base power and toughness become 4/1 or 1/4 until end of turn</examplecapture>
[Dependent]
public class MasterOfWindsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sorcery, or wizard spell, you may have this creature's base power and toughness become 4/1 or 1/4 until end of turn"];
}
