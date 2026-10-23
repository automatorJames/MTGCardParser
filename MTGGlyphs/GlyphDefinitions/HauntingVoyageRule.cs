namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haunting Voyage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunting Voyage</exampledoc>
[TokenizationOrder(-1)]
public class HauntingVoyageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was foretold, return all creature cards of that type from your graveyard to the battlefield instead"];
}
