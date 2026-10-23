namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Strategic Betrayal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strategic Betrayal</exampledoc>
[TokenizationOrder(-1)]
public class StrategicBetrayalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent exiles a creature they control and their graveyard"];
}
