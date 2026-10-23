namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rescue from the Underworld, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rescue from the Underworld</exampledoc>
[TokenizationOrder(-1)]
public class RescueFromTheUnderworldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card and the sacrificed card to the battlefield under your control at the beginning of your next upkeep"];
}
