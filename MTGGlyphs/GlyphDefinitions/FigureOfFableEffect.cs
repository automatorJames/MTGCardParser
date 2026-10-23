namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Figure of Fable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Figure of Fable</exampledoc>
/// <examplecapture>if this creature is a soldier, it becomes a kithkin avatar with base power and toughness 7/8 and protection from each of your opponents</examplecapture>
[Dependent]
public class FigureOfFableEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is a soldier, it becomes a kithkin avatar with base power and toughness 7/8 and protection from each of your opponents"];
}
