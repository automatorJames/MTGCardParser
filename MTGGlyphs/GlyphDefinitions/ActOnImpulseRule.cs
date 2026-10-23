namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Act on Impulse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Act on Impulse</exampledoc>
/// <examplecapture>you can play a land this way only if you have an available land play remaining</examplecapture>
[TokenizationOrder(-1)]
public class ActOnImpulseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can play a land this way only if you have an available land play remaining"];
}
