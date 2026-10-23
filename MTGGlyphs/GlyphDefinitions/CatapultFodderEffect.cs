namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Catapult Fodder // Catapult Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Catapult Fodder // Catapult Captain</exampledoc>
/// <examplecapture>if you control three or more creatures that each have toughness greater than their power, transform this creature</examplecapture>
[Dependent]
public class CatapultFodderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control three or more creatures that each have toughness greater than their power, transform this creature"];
}
