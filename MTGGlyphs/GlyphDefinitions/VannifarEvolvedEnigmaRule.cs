namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vannifar, Evolved Enigma, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vannifar, Evolved Enigma</exampledoc>
[TokenizationOrder(-1)]
public class VannifarEvolvedEnigmaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• cloak a card from your hand"];
}
