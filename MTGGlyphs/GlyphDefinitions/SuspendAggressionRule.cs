namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Suspend Aggression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suspend Aggression</exampledoc>
[TokenizationOrder(-1)]
public class SuspendAggressionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target nonland permanent and the top card of your library"];
}
