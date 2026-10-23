namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tishana's Tidebinder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tishana's Tidebinder</exampledoc>
/// <examplecapture>if an ability of an artifact, creature, or planeswalker is countered this way, that permanent loses all abilities for as long as this creature remains on the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class TishanasTidebinderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an ability of an artifact, creature, or planeswalker is countered this way, that permanent loses all abilities for as long as this creature remains on the battlefield"];
}
