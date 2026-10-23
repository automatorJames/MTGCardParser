namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Qarsi Deceiver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Qarsi Deceiver</exampledoc>
/// <examplecapture>spend this mana only to cast a face-down creature spell, pay a mana cost to turn a manifested creature face up, or pay a morph cost</examplecapture>
[TokenizationOrder(-1)]
public class QarsiDeceiverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast a face-down creature spell, pay a mana cost to turn a manifested creature face up, or pay a morph cost"];
}
