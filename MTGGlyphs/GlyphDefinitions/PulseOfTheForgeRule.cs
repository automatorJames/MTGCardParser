namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pulse of the Forge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pulse of the Forge</exampledoc>
[TokenizationOrder(-1)]
public class PulseOfTheForgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if that player or that planeswalker's controller has more life than you, return", Nib.This, "to its owner's hand"];
}
