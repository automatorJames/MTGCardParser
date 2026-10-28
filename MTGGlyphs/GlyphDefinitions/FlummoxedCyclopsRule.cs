namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flummoxed Cyclops, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flummoxed Cyclops</exampledoc>
[TokenizationOrder(-1)]
public class FlummoxedCyclopsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever two or more creatures your opponents control attack, this creature can't block this combat"];
}
