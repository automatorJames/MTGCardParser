namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shape Stealer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shape Stealer</exampledoc>
/// <examplecapture>this creature's base power and toughness become equal to that creature's power and toughness until end of turn</examplecapture>
[Dependent]
public class ShapeStealerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's base power and toughness become equal to that creature's power and toughness until end of turn"];
}
